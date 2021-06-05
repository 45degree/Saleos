using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saleos.Entity;
using Saleos.Entity.Data;

namespace Saleos
{
    public static class SeedData
    {
        /// <summary>
        /// this function will migrate the core database and identity database
        /// </summary>
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var homePageDbContext = scope.ServiceProvider.GetService<HomePageDbContext>();
            var identityDbContext = scope.ServiceProvider.GetService<HomePageIdentityDbContext>();
            if (homePageDbContext == null || identityDbContext == null)
            {
                throw new Exception("Can't get the context");
            }

            await homePageDbContext.Database.EnsureCreatedAsync();
            await homePageDbContext.Database.MigrateAsync();

            await identityDbContext.Database.EnsureCreatedAsync();
            await identityDbContext.Database.MigrateAsync();

            var category1 = new Category()
            {
                Content = "Category 1"
            };
            
            var article1 = new Article()
            {
                Title =  "Title 1",
                Content = "Content 1",
                CreateTime = new DateTime(2020, 2, 1),
                LastModifiedTime = DateTime.Today,
                Category = category1
            };

            var tag1 = new Tag()
            {
                Content = "Tag 1",
            };

            var articleTag1 = new ArticleTag {Article = article1, Tag = tag1};

            homePageDbContext.Article.AddRange(article1);
            homePageDbContext.Tags.AddRange(tag1);
            homePageDbContext.Categories.AddRange(category1);
            await homePageDbContext.AddRangeAsync(articleTag1);

            await homePageDbContext.SaveChangesAsync();

            // create admin
            var username = configuration["Admin:Username"];
            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];
        }
    }
}