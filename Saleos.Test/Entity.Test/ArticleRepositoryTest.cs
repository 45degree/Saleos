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
using Saleos.DTO;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Entity.Test
{
    public abstract class ArticleRepositoryTest : BaseServicesTest
    {
        protected ArticleRepositoryTest(DbContextOptions<HomePageDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        [Fact]
        public async Task ArticleIsExisted_IdIsInRange_ReturnTrue()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.True(await articleServices.ArticleRepository.ArticleIsExisted(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task ArticleIsExisted_IdIsOutOfRange_ReturnFalse(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.False(await articleServices.ArticleRepository.ArticleIsExisted(articleId));
        }

        [Fact]
        public async void GetArticleAsync_IdIsInRange_ReturnArticle()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var article1 = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(_mockData.Articles[0].Id, article1.Id);
            Assert.Equal(_mockData.Articles[0].Title, article1.Title);
            Assert.Equal(_mockData.Articles[0].Content, article1.Content);
            Assert.Equal(_mockData.Articles[0].Abstract, article1.Abstract);
            Assert.Equal(_mockData.Articles[0].Category.Content, article1.Category.Content);
            Assert.Equal(_mockData.ArticleTags.FindAll(x => x.ArticleId == 1).Count,
                article1.Tags.Count);

            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(_mockData.Articles[1].Id, article2.Id);
            Assert.Equal(_mockData.Articles[1].Title, article2.Title);
            Assert.Equal(_mockData.Articles[1].Content, article2.Content);
            Assert.Equal(_mockData.Articles[1].Abstract, article2.Abstract);
            Assert.Equal(_mockData.Articles[1].Category.Content, article2.Category.Content);
            Assert.Equal(_mockData.ArticleTags.FindAll(x => x.ArticleId == 2).Count,
                article2.Tags.Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetArticleAsync_IdIsOutOfRange_ThrowException(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.ArticleRepository.GetArticleAsync(articleId));
        }

        [Theory]
        [InlineData(4)]
        public async Task GetArticleAsync_ValidIdIsNotInDataBase_ReturnNull(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var article = await articleServices.ArticleRepository.GetArticleAsync(articleId);
            Assert.Null(article);
        }

        [Fact]
        public async Task GetArticleByQueryAsync_SimplePaging_GetArticles()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                PageNumber = 1,
                PageSize = 2,
            };
            var articles = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Equal(2,articles.Count);
            Assert.Equal(_mockData.Articles[0].Id, articles[0].Id);
            Assert.Equal(_mockData.Articles[1].Id, articles[1].Id);

            queryDto.PageNumber = 2;
            articles = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Single(articles);
            Assert.Equal(_mockData.Articles[2].Id, articles[0].Id);
        }

        [Fact]
        public async Task GetArticleByQueryAsync_PageNumberIsOutOfRange_GetEmpty()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                PageNumber = 2,
                PageSize = 3,
            };
            var articles = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Empty(articles);
        }


        [Fact]
        public async void GetArticleCountAsync_ReturnArticleCount()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var count = await articleServices.ArticleRepository.GetArticleCountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task AddArticle_ArticleIsValid_ArticleIsRestoredInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var newArticle = new ArticleAddDto()
            {
                Title = "Title 4",
                Content = "Content 4",
                CategoryId = 2,
                CreateTime = new DateTime(2020, 2, 3),
                IsReprint = true,
                ReprintUri = "https://new.com"
            };
            await articleServices.ArticleRepository.AddArticleAsync(newArticle);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(4);
            Assert.Equal(4, article.Id);
            Assert.Equal("Title 4", article.Title);
            Assert.Equal("Content 4", article.Content);
            Assert.Equal("Category 2", article.Category.Content);
            Assert.True(article.IsReprint);
            Assert.Equal("https://new.com", article.RerpintUri);
        }

        [Fact]
        public async Task AddArticle_ArticleIsNull_ExceptionThrow()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                articleServices.ArticleRepository.AddArticleAsync(null));
        }

        [Fact]
        public async Task UpdateArticle_ArticleChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var articleUpdate = new ArticleUpdateDto()
            {
                Id = 3,
                Content = "Changed Content 3",
                Tags = new List<int>(){1, 2, 3 },
                CategoryId = 1,
                IsReprint = true,
                ReprintUri = "https://new.com",
            };
            await articleServices.ArticleRepository.UpdateArticleAsync(articleUpdate);
            await articleServices.SaveAsync();

            var newArticle = await articleServices.ArticleRepository.GetArticleAsync(3);
            Assert.Equal("Changed Content 3", newArticle.Content);
            Assert.Equal(3, newArticle.Tags.Count);
            Assert.Equal("Category 1", newArticle.Category.Content);
            Assert.True(newArticle.IsReprint);
            Assert.Equal("https://new.com", newArticle.RerpintUri);
        }

        [Fact]
        public async Task DeleteArticle_IdIsInRange_RemoveArticleAndArticleTagsInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.ArticleRepository.DeleteArticleAsync(1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Null(article);

            Assert.Equal(3, context.ArticleTags.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task DeleteArticle_IdIsOutOfRange_ExceptionThrow(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.ArticleRepository.DeleteArticleAsync(articleId));
        }
    }
}
