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

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saleos.Entity;
using Saleos.Entity.Data;

namespace Saleos.Admin
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
            if (homePageDbContext == null)
            {
                throw new Exception("Can't get the context");
            }

            if(!await homePageDbContext.Database.EnsureCreatedAsync()) return;
            await homePageDbContext.Database.MigrateAsync();

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
