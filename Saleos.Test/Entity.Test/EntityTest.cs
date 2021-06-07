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

namespace Saleos.Test.Entity.Test
{
    /// <summary>
    /// 这个类主要用于
    /// 1. 测试EF Core的功能是否正常
    /// 2. 测试Entity的表示是否正确
    /// </summary>
    public abstract class EntityTest
    {
        private DbContextOptions<HomePageDbContext> ContextOptions { get; }
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
            Assert.Equal("Tag 1", tag.Content);

            tag = context.Tags.SingleOrDefault(x => x.Id == 2);
            Assert.NotNull(tag);
            Assert.Equal("Tag 2", tag.Content);

            // test article
            var article = context.Article.SingleOrDefault(x => x.Id == 1);
            Assert.NotNull(article);
            Assert.Equal("Content 1", article.Content);
            Assert.Equal("Abstract 1", article.Abstract);
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
            Assert.Equal("Tag 1", tag.Content);
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
            Assert.Equal("Content 1", article.Content);
        }

        /// <summary>
        /// generator test data in the database
        /// </summary>
        public static void SeedDate(HomePageDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var category1 = new Category()
            {
                Content = "Category 1"
            };

            var category2 = new Category()
            {
                Content = "Category 2"
            };

            var article1 = new Article()
            {
                Title =  "Title 1",
                Abstract = "Abstract 1",
                Content = "Content 1",
                ImageUrl = "Url 1",
                Category = category1,
                CreateTime = new DateTime(2020, 2, 1),
                LastModifiedTime = DateTime.Today
            };

            var article2 = new Article()
            {
                Title =  "Title 2",
                Abstract = "Abstract 2",
                Content = "Content 2",
                ImageUrl = "Url 2",
                Category = category1,
                CreateTime = new DateTime(2020, 2, 2),
                LastModifiedTime = DateTime.Today
            };

            var article3 = new Article()
            {
                Title =  "Title 3",
                Abstract = "Abstract 3",
                Content = "Content 3",
                ImageUrl = "Url 2",
                Category = category2,
                CreateTime = new DateTime(2020, 2, 3),
                LastModifiedTime = DateTime.Today
            };

            var tag1 = new Tag()
            {
                Content = "Tag 1",
            };
            var tag2 = new Tag()
            {
                Content = "Tag 2",
            };
            var tag3 = new Tag()
            {
                Content = "Tag 3",
            };

            var articleTag1 = new ArticleTag {Article = article1, Tag = tag1};
            var articleTag2 = new ArticleTag {Article = article1, Tag = tag2};
            var articleTag3 = new ArticleTag {Article = article2, Tag = tag2};
            var articleTag4 = new ArticleTag {Article = article1, Tag = tag3};
            var articleTag5 = new ArticleTag {Article = article2, Tag = tag3};
            var articleTag6 = new ArticleTag {Article = article3, Tag = tag3};

            context.Article.AddRange(article1, article2, article3);
            context.Tags.AddRange(tag1, tag2, tag3);
            context.Categories.AddRange(category1, category2);
            context.AddRange(articleTag1, articleTag2,
                articleTag3, articleTag4,
                articleTag5, articleTag6);

            context.SaveChanges();
        }
    }
}