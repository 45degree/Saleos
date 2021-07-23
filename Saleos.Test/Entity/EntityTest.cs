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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.Entity;
using Saleos.Entity.Data;
using Xunit;

namespace Saleos.Test.Entity
{
    /// <summary>
    /// 这个类主要用于
    /// 1. 测试EF Core的功能是否正常
    /// 2. 测试Entity的表示是否正确
    /// </summary>
    public abstract class EntityTest
    {
        private DbContextOptions<HomePageDbContext> homePageContextOptions { get; }
        private DbContextOptions<IdentityDbContext> identityContextOptions { get; }
        private readonly MockData _mockData = MockData.GetInstance();

        protected EntityTest(DbContextOptions<HomePageDbContext> homePageContextOptions = null,
            DbContextOptions<IdentityDbContext> identityContextOptions = null)
        {
            this.homePageContextOptions = homePageContextOptions;
            this.identityContextOptions = identityContextOptions;

            HomePageDbContext homePageContext = null;
            if(homePageContextOptions != null)
            {
                homePageContext = new HomePageDbContext(homePageContextOptions);
            }

            IdentityDbContext identityContext = null;
            if(identityContextOptions != null)
            {
                identityContext = new IdentityDbContext(identityContextOptions);
            }

            MockData.SeedData(homePageContext, identityContext);

            if(homePageContext != null)
            {
                homePageContext.Dispose();
            }
            if(identityContext != null)
            {
                identityContext.Dispose();
            }
        }

        [Fact]
        public void GetItems()
        {
            if(homePageContextOptions == null) return;

            // test tag
            using var context = new HomePageDbContext(homePageContextOptions);
            var tag = context.Tags.SingleOrDefault(x => x.Id == 1);
            Assert.NotNull(tag);
            Assert.Equal(_mockData.Tags[0].Content, tag.Content);

            tag = context.Tags.SingleOrDefault(x => x.Id == 2);
            Assert.NotNull(tag);
            Assert.Equal(_mockData.Tags[1].Content, tag.Content);

            // test article
            var article = context.Article.SingleOrDefault(x => x.Id == 1);
            Assert.NotNull(article);
            Assert.Equal(_mockData.Articles[0].Content, article.Content);
            Assert.Equal(_mockData.Articles[0].Abstract, article.Abstract);
        }

        [Fact]
        public void GetTagsFromArticle()
        {
            if(homePageContextOptions == null) return;

            using var context = new HomePageDbContext(homePageContextOptions);
            var article = context.Article
                .Where(x => x.Id == 1)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault();

            Assert.NotNull(article);
            var tagId = article.ArticleTags[0].TagId;
            var tag = article.ArticleTags[0].Tag;
            Assert.Equal(1, tagId);
            Assert.Equal(_mockData.Tags[0].Content, tag.Content);
        }

        [Fact]
        public void GetArticleFromTag()
        {
            if(homePageContextOptions == null) return;

            using var context = new HomePageDbContext(homePageContextOptions);
            var tag = context.Tags
                .Where(x => x.Id == 1)
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .FirstOrDefault();

            Assert.NotNull(tag);
            var articleId = tag.ArticleTag[0].ArticleId;
            var article = tag.ArticleTag[0].Article;
            Assert.Equal(1, articleId);
            Assert.Equal(_mockData.Articles[0].Content, article.Content);
        }

        [Fact]
        public async Task GetRoles()
        {
            if(identityContextOptions == null) return;

            using var context = new IdentityDbContext(identityContextOptions);
            var roles = await context.Roles.ToListAsync();

            var mockData = MockData.GetInstance();

            Assert.Equal(mockData.Roles.Count, roles.Count);
            for(var i = 0; i < mockData.Roles.Count; i ++)
            {
                Assert.Equal(mockData.Roles[i].RoleName, roles[i].RoleName);
                Assert.Equal(mockData.Roles[i].Id, roles[i].Id);
            }
        }

        [Fact]
        public async Task GetUsers()
        {
            if(identityContextOptions == null) return;

            using var context = new IdentityDbContext(identityContextOptions);
            var users = await context.Users.Include(x => x.Roles).ToListAsync();

            var mockData = MockData.GetInstance();

            Assert.Equal(mockData.Users.Count, users.Count);
            for(var i = 0; i < mockData.Users.Count; i++)
            {
                Assert.Equal(mockData.Users[i].Id, users[i].Id);
                Assert.Equal(mockData.Users[i].Username, users[i].Username);
                for(var j = 0; j < mockData.Users[i].Roles.Count; j++)
                {
                    Assert.Equal(mockData.Users[i].Roles[j].Id, users[i].Roles[j].Id);
                    Assert.Equal(mockData.Users[i].Roles[j].RoleName, users[i].Roles[j].RoleName);
                }
            }
        }
    }
}
