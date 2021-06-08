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
    public abstract class CategoryRepositoryTest : BaseServicesTest
    {
        protected CategoryRepositoryTest(DbContextOptions<HomePageDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task CategoryIsExist_IdIsInRange_ReturnTrue(int categoryId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.True(await articleServices.CategoryRepository.CategoryIsExistAsync(categoryId));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task CategoryIsExist_IdIsOutOfRange_ReturnFalse(int categoryId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.False(await articleServices.CategoryRepository.CategoryIsExistAsync(categoryId));
        }

        [Fact]
        public async Task GetCategoryAsync_ReturnAllCategory()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var categories= await articleServices.CategoryRepository.GetCategoryAsync();
            Assert.Equal(_mockData.Categories.Count, categories.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task GetCategoryAsync_IdIsInRange_ReturnCategory(int categoryId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var categoryDto = await articleServices.CategoryRepository.GetCategoryAsync(categoryId);
            Assert.Equal(categoryId, categoryDto.Id);
            Assert.Equal(_mockData.Categories[categoryId - 1].Content, categoryDto.Content);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task GetCategoryAsync_IdIsOutOfRange_ExceptionThrow(int categoryId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.CategoryRepository.GetCategoryAsync(categoryId));
        }

        [Fact]
        public async Task GetCategoryCountAsync_ReturnCategoriesCount()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var count = await articleServices.CategoryRepository.GetCategoryCountAsync();
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task AddCategory_CategoryIsValid_CategoryRestoredInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var newCategory = new CategoryAddDto()
            {
                Content = "Category 4"
            };
            await articleServices.CategoryRepository.AddCategoryAsync(newCategory);
            await articleServices.SaveAsync();

            var categoryDto = await articleServices.CategoryRepository.GetCategoryAsync(3);
            Assert.Equal("Category 4", categoryDto.Content);
        }

        [Fact]
        public async Task AddCategory_CategoryIsNull_ExceptionThrow()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<ArgumentNullException>(() => articleServices.CategoryRepository.AddCategoryAsync(null));
        }

        [Fact]
        public async Task UpdateCategory_CategoryChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var updateCategory = new CategoryUpdateDto()
            {
                Id = 1,
                Content = "Changed Category 1",
            };
            await articleServices.CategoryRepository.UpdateCategoryAsync(updateCategory);
            await articleServices.SaveAsync();

            var newCategory = await articleServices.CategoryRepository.GetCategoryAsync(1);
            Assert.Equal("Changed Category 1", newCategory.Content);
        }

        [Fact]
        public async Task DeleteCategory_IdIsInRange_RemoveCategoryAndArticleCategoryInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.CategoryRepository.DeleteCategoryAsync(1);
            await articleServices.SaveAsync();

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.CategoryRepository.GetCategoryAsync(1));

            // whether the article that has this tag really delete this tag in database
            var article1 = await articleServices.ArticleRepository.GetArticleAsync(1);
            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            var article3 = await articleServices.ArticleRepository.GetArticleAsync(3);
            Assert.Null(article1.Category);
            Assert.Null(article2.Category);
            Assert.NotNull(article3.Category);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task DeleteCategory_IdIsOutOfRange_ExceptionThrow(int categoryId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.CategoryRepository.DeleteCategoryAsync(categoryId));
        }

        [Fact]
        public async Task GetCategoryByQueryAsync_TitleIsInData_ReturnArticle()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new CategoryQueryDto()
            {
                Content = "Category 1",
            };
            var categories = await articleServices.CategoryRepository.GetCategoryByQueryAsync(queryDto);
            Assert.Single(categories);
            Assert.Equal("Category 1", categories[0].Content);
        }

        [Theory]
        [InlineData("nonexistent title")]
        [InlineData("Category 1 ")]
        public async Task GetCategoryByQueryAsync_TitleIsNotInData_ReturnNull(string content)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new CategoryQueryDto()
            {
                Content = content,
            };
            var categories = await articleServices.CategoryRepository.GetCategoryByQueryAsync(queryDto);
            Assert.Empty(categories);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetCategoryByQueryAsync_TitleIsNullOrWhiteSpace_IgnoreTitleFilter(string content)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDto = new CategoryQueryDto()
            {
                Content = content,
            };
            var categories = await articleServices.CategoryRepository.GetCategoryByQueryAsync(queryDto);
            Assert.Equal(_mockData.Categories.Count, categories.Count);
        }

        //TODO test query string in pagination
    }
}
