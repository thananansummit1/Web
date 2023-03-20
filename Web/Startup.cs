using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Util;

namespace Web
{ 
        public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
                // Swagger
                const string buildNumber = "build_number";

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Test API",
                        Version = "v1",
                        Description = "Test API blueprint<br>" +
                            "Version: v" + GetType().Assembly.GetName().Version.ToString() + "." + buildNumber + "<br>",
                        //"Environment: " + this.EnvironmentName,
                        Contact = new OpenApiContact
                        {
                            Name = "dev_team",
                            Email = "thananan.t@summitcapital.co.th",
                        }
                    });
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.Last());


                    var securityScheme = new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                    };
                    c.AddSecurityDefinition("Bearer", securityScheme);

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      Array.Empty<string>()
                    }
                  });

                    // csv
                    c.MapType<FileContentResult>(() => new OpenApiSchema
                    {
                        Type = "file"
                    });
                });

                services.AddHealthChecks();
                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

                services.AddCors(options =>
                {
                    options.AddPolicy("SummitCapitalCore",
                        builder =>
                        {
                            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                        }
                        );

                });


                // Get latest version
                JWT.Version = GetType().Assembly.GetName().Version.ToString();

                //var address = Configuration.GetValue<string>("ConnectionStrings:ChatbotConnection");
                //if (address.Split(";")[0].Length > 0)
                //{
                //    JWT.BuildVersion = address.Split(";")[0].Split("=").Last();
                //}

                // service
                //services.AddTransient<ILoginService, LoginService>();


                // Repository

                // db
                services.AddControllers();
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Channel");
                });

                app.UseHttpsRedirection();

                //app.UseStaticFiles(new StaticFileOptions
                //{
                //    FileProvider = new PhysicalFileProvider(
                //        Path.Combine(Directory.GetCurrentDirectory(),
                //        "wwwroot"
                //    ))
                //});

                app.UseRouting();

                app.UseCors("SummitCapitalCore");

                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }

}