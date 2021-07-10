/*
 * Copyright 2021 45degree
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Saleos.Entity.Services.ImageStorage;

namespace Saleos
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
            services.AddDbContext<HomePageDbContext>(options =>
                options.UseNpgsql(
                    string.Format(Configuration.GetConnectionString("DefaultConnection"),
                    Configuration["POSTGRES_HOST"] ?? "localhost",
                    Configuration["POSTGRES_PORT"] ?? "5432",
                    Configuration["POSTGRES_USER"] ?? "Saleos",
                    Configuration["POSTGRES_PASSWORD"] ?? "Saleos")
                )
            );
            services.AddScoped<IImageStorage, MinioImageStorage>();
            services.AddScoped<ArticleServices, ArticleServicesImpl>();

            /*
             * config Cors
             */

            services.AddCors(options =>
            {
               options.AddPolicy("Open", builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Saleos", Version = "v1" });
            });

        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Saleos v1"));
            }

            // app.UseHttpsRedirection();

            app.UseCors("Open");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
