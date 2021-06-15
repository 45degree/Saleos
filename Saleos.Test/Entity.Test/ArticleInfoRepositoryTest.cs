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
using Saleos.DTO;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Entity.Test
{
    public abstract class ArticleInfoRepositoryTest : BaseServicesTest
    {
        protected ArticleInfoRepositoryTest(DbContextOptions<HomePageDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        [Fact]
        public async Task GetArticleInfoByQueryAsync_TitleIsInData_ReturnArticle()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var realArticle = _mockData.Articles[0];
            var queryDto = new ArticlesQueryDto()
            {
                Title = realArticle.Title,
            };
            var article = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Single(article);
            Assert.Equal(realArticle.Id, article[0].Id);
            Assert.Equal(realArticle.Abstract, article[0].Abstract);
            Assert.Equal(_mockData.ArticleTags.FindAll(x => x.ArticleId == realArticle.Id).Count,
                article[0].Tags.Count);
        }

        [Theory]
        [InlineData("nonexistent title")]
        [InlineData("Title 1 ")]
        public async Task GetArticleInfoByQueryAsync_TitleIsNotInData_ReturnNull(string title)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                Title = title,
            };
            var article = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Empty(article);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetArticleInfoByQueryAsync_TitleIsNullOrWhiteSpace_IgnoreTitleFilter(
            string title)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                Title = title,
            };
            var articles = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Equal(_mockData.Articles.Count, articles.Count);
        }

        [Fact]
        public async Task GetAllArticleInfo_GetAllArticleInfo()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);

            var articles = await articleServices.ArticleInfoRepository.GetAllArticleInfo();
            Assert.Equal(_mockData.Articles.Count, articles.Count);
            Assert.Equal(_mockData.ArticleTags.FindAll(x => x.ArticleId == 1).Count,
                articles[0].Tags.Count);
        }

        [Fact]
        public async Task GetArticleInfoByQueryAsync_SimplePaging_GetArticles()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                PageNumber = 1,
                PageSize = 2,
            };
            var articles = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Equal(2,articles.Count);
            Assert.Equal(_mockData.Articles[0].Id, articles[0].Id);
            Assert.Equal(_mockData.Articles[1].Id, articles[1].Id);

            queryDto.PageNumber = 2;
            articles = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Single(articles);
            Assert.Equal(_mockData.Articles[2].Id, articles[0].Id);
            Assert.Single(articles);
        }

        [Fact]
        public async Task GetArticleInfoByQueryAsync_PageNumberIsOutOfRange_GetEmpty()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new ArticlesQueryDto()
            {
                PageNumber = 2,
                PageSize = 3,
            };
            var articles = await articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            Assert.Empty(articles);
        }
    }
}
