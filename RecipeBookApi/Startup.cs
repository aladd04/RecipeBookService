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

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var googleAuthSecret = Configuration.GetValue<string>("GoogleAuthSecret");

                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(googleAuthSecret))
                    };
                });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var corsOrigin = Configuration.GetValue<string>("CorsOrigin");

                    builder.WithOrigins(corsOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
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
            services.AddScoped<IAuthService, GoogleAuthService>();
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

            app.UseAuthentication();
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}