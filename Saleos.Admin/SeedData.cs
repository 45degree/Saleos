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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saleos.Entity;
using Saleos.Entity.Data;
using Saleos.Entity.Services.IdentityService;

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

            if(await homePageDbContext.Database.EnsureCreatedAsync())
            {
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
            }

            // create admin
            var username = configuration["SALEOS_ADMIN_NAME"];
            var password = configuration["SALEOS_ADMIN_PASSWORD"];
            var salt = GetRandomString(5, true, true, true, false, null);

            var passwordHash = new PasswordHash();
            var passwordSha1 = await passwordHash.GetPasswordHashAsync(password, salt);

            var identityServices = scope.ServiceProvider.GetService<IdentityDbContext>();
            if (identityServices == null)
            {
                throw new Exception("Can't get the context");
            }

            if(await identityServices.Database.EnsureCreatedAsync())
            {
                await identityServices.Database.MigrateAsync();

                if(!await identityServices.Roles.AnyAsync(x => x.RoleName == "Admin"))
                {
                    identityServices.Roles.Add(new Role
                    {
                        RoleName = "Admin"
                    });
                    await identityServices.SaveChangesAsync();
                }
                var role = await identityServices.Roles
                    .SingleOrDefaultAsync(x => x.RoleName == "Admin");

                identityServices.Users.Add(new User
                {
                    Username = username,
                    PasswordSha1 = passwordSha1,
                    Roles = new List<Role> { role },
                    Salt = salt,
                    CreateTime = DateTime.Now,
                });

                await identityServices.SaveChangesAsync();
            }

        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        /// <returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
