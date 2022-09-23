using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecipeBookApi.Dynamo;
using RecipeBookApi.Dynamo.Contracts;
using RecipeBookApi.Dynamo.Models;
using RecipeBookApi.Options;
using RecipeBookApi.Services;
using RecipeBookApi.Services.Contracts;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
#if DEBUG
builder.Logging.AddDebug();
#endif

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddOptions<AppOptions>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddTransient<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddTransient<IDynamoStorageRepository<AppUser>, DynamoStorageRepository<AppUser>>();
builder.Services.AddTransient<IDynamoStorageRepository<Recipe>, DynamoStorageRepository<Recipe>>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();
builder.Services.AddTransient<IAuthService, GoogleAuthService>();
builder.Services.AddTransient<IRecipeService, DynamoRecipeService>();

var appOptions = builder.Configuration.Get<AppOptions>();

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .WithOrigins(appOptions.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.SaveToken = true;

        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appOptions.GoogleClientSecret))
        };
    });

builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "Recipe Book API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI(swaggerUiOptions =>
{
    swaggerUiOptions.SwaggerEndpoint($"{(app.Environment.IsDevelopment() ? string.Empty : "/Prod")}/swagger/v1/swagger.json", "Recipe Book API");
});

app.UseCors();
app.UseHsts();
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseEndpoints(endpointBuilder =>
{
    endpointBuilder.MapControllers();
});

await app.RunAsync();
