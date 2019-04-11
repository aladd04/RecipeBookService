using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Common.Dynamo;
using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RecipeBookApi.Services;
using RecipeBookApi.Services.Contracts;
using RecipeBookApi.TokenValidators;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

namespace RecipeBookApi
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Recipe Book API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "http://katiemaeskitchen.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var googleAuthSecret = Configuration.GetValue<string>("GoogleAuthSecret");
                var googleClientId = Configuration.GetValue<string>("GoogleClientId");
                var authority = "accounts.google.com";

                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.Authority = authority;
                options.Audience = googleClientId;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authority,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(googleAuthSecret))
                };

                options.SecurityTokenValidators.Clear();
                options.SecurityTokenValidators.Add(new GoogleTokenValidator());
            });

            services.AddScoped<IDynamoDBContext, DynamoDBContext>(serviceProvider =>
            {
                var awsOptions = Configuration.GetAWSOptions();
                var awsAccessKey = Configuration.GetValue<string>("AWSAccessKey");
                var awsSecretAccessKey = Configuration.GetValue<string>("AWSSecretAccessKey");

                return new DynamoDBContext(new AmazonDynamoDBClient(awsAccessKey, awsSecretAccessKey, awsOptions.Region));
            });

            services.AddScoped<IDynamoStorageRepository<AppUser>, DynamoStorageRepository<AppUser>>();
            services.AddScoped<IDynamoStorageRepository<Recipe>, DynamoStorageRepository<Recipe>>();
            services.AddScoped<IAppUserService, DynamoAppUserLogic>();
            services.AddScoped<IRecipeService, DynamoRecipeService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recipe Book API");
            });

            app.UseCors();
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}