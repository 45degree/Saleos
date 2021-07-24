using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Saleos.Entity.Services.IdentityService;
using Saleos.Entity.Services.ImageStorage;

namespace Saleos.Admin
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
            services.AddControllersWithViews().AddNewtonsoftJson();

            // Add Database
            services.AddDbContext<HomePageDbContext>(options =>
                options.UseNpgsql(
                    string.Format(Configuration.GetConnectionString("CoreConnection"),
                    Configuration["POSTGRES_HOST"] ?? "localhost",
                    Configuration["POSTGRES_PORT"] ?? "5432",
                    Configuration["POSTGRES_USER"] ?? "Saleos",
                    Configuration["POSTGRES_PASSWORD"] ?? "Saleos")
                )
            );

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(
                    string.Format(Configuration.GetConnectionString("IdentityConnection"),
                    Configuration["POSTGRES_HOST"] ?? "localhost",
                    Configuration["POSTGRES_PORT"] ?? "5432",
                    Configuration["POSTGRES_USER"] ?? "Saleos",
                    Configuration["POSTGRES_PASSWORD"] ?? "Saleos")
                )
            );

            services.AddScoped<IImageStorage, MinioImageStorage>();
            services.AddScoped<IPasswordHash, PasswordHash>();
            services.AddScoped<IIdentityService, IdentityServiceImpl>();
            services.AddScoped<ArticleServices, ArticleServicesImpl>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/login");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Article}/{action=Index}"
                );
            });
        }
    }
}
