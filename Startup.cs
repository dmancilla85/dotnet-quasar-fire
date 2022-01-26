using FuegoDeQuasar.Configuration;
using FuegoDeQuasar.Services;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace FuegoDeQuasar
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the
        // HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => c.SerializeAsV2 = true);
                _ = app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Configuration["Swagger:Name"]} v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseProblemDetails();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        // This method gets called by the runtime. Use this method to add services
        // to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<SatellitesOptions>(Configuration.GetSection(
                                        SatellitesOptions.SatellitesConfiguration));

            services.AddProblemDetails()
                    .AddControllersWithViews()
                    .AddProblemDetailsConventions();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = $"{Configuration["Swagger:Version"]}",
                    Title = $"{Configuration["Swagger:Title"]}",
                    Description = $"{Configuration["Swagger:Description"]}",
                    Contact = new OpenApiContact
                    {
                        Name = $"{Configuration["Swagger:Name"]}",
                        Email = $"{Configuration["Swagger:Email"]}",
                        Url = new Uri($"{Configuration["Swagger:GitHub"]}"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<ITopSecretService, TopSecretService>();
        }
    }
}