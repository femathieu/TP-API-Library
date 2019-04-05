using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace LibraryAPI
{
    public class Startup
    {
        private readonly AppConfig _Config = new AppConfig();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddDbContext<LibraryContext>(options => options.UseSqlServer(
                "Server=DESKTOP-P4NO1RB\\SQLEXPRESS;Database=Library;Trusted_Connection=True;ConnectRetryCount=0"
            ));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "LibraryAPI",
                    new Info
                    {
                        Title = "Library API",
                        Version = "v1"
                    });
                var basePath = System.AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "LibraryAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
                .AddJwtBearer("JwtBearer", jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _Config.getSecretPassPhrase(),
                        ValidateLifetime = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/LibraryAPI/swagger.json",
                    "Library API V1"
                );
            });

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
