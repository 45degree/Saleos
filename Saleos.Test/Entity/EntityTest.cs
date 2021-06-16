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
        private DbContextOptions<HomePageDbContext> ContextOptions { get; }
        private MockData _mockData = MockData.getInstance();

        protected EntityTest(DbContextOptions<HomePageDbContext> contextOptions)
        {
            this.ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using var context = new HomePageDbContext(ContextOptions);
            SeedDate(context);
        }

        [Fact]
        public void GetItems()
        {
            // test tag
            using var context = new HomePageDbContext(ContextOptions);
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
            using var context = new HomePageDbContext(ContextOptions);
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
            using var context = new HomePageDbContext(ContextOptions);
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

        /// <summary>
        /// generator test data in the database
        /// </summary>
        public static void SeedDate(HomePageDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var mockData = MockData.getInstance();

            context.Article.AddRange(mockData.Articles);
            context.Tags.AddRange(mockData.Tags);
            context.Categories.AddRange(mockData.Categories);
            context.ArticleTags.AddRange(mockData.ArticleTags);
            context.SaveChanges();
        }
    }
}
