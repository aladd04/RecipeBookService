using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Common.Dynamo;
using Common.Dynamo.Contracts;
using Common.Dynamo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeBookApi.Logic;
using RecipeBookApi.Logic.Contracts;
using Swashbuckle.AspNetCore.Swagger;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Recipe Book API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var awsOptions = Configuration.GetAWSOptions();

            services.AddScoped<IDynamoDBContext, DynamoDBContext>(serviceProvider =>
            {
                var awsAccessKey = Configuration.GetValue<string>("AWSAccessKey");
                var awsSecretAccessKey = Configuration.GetValue<string>("AWSSecretAccessKey");

                return new DynamoDBContext(new AmazonDynamoDBClient(awsAccessKey, awsSecretAccessKey, awsOptions.Region));
            });

            services.AddScoped<IDynamoStorageRepository<AppUser>, DynamoStorageRepository<AppUser>>();
            services.AddScoped<IDynamoStorageRepository<Recipe>, DynamoStorageRepository<Recipe>>();
            services.AddScoped<IAppUserLogic, DynamoAppUserLogic>();
            services.AddScoped<IRecipeLogic, DynamoRecipeLogic>();
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
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}