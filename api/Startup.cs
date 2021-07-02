using api.Database;
using api.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace api
{
    public class Startup
    {
        private readonly string OrigensAceitas = "OrigensAceitas";
        public static readonly string[] Origins = new string[] { "http://localhost:49959", "https://localhost:49959" };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddCors(options =>
            {
                options.AddPolicy(OrigensAceitas, 
                    builder =>
                    {
                        builder.WithOrigins(Origins)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed(x => _ = true)
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowCredentials();
                    });
            });

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
            });
            services.AddAuthorization();
            services.AddResponseCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));

                Thread.Sleep(10000);
                SQLStartup.ConfigureDatabase();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(OrigensAceitas);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(OrigensAceitas);
                endpoints.MapControllerRoute("default", "{controller}/{action}");
            });

            app.UseResponseCompression();
        }
    }
}
