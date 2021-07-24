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
using Saleos.DAO;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Entity.CoreServicesTest
{
    public abstract class TagRepositoryTest : BaseCoreServicesTest
    {
        protected TagRepositoryTest(DbContextOptions<HomePageDbContext> contextOptions)
            : base(contextOptions)
        {
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task TagIsExisted_IdIsInRange_ReturnTrue(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.True(await articleServices.TagRepository.TagIsExistAsync(articleId));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task TagIsExisted_IdIsOutOfRange_ReturnFalse(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.False(await articleServices.TagRepository.TagIsExistAsync(articleId));
        }

        [Fact]
        public async Task GetTagAsync_ReturnAllTags()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var tags= await articleServices.TagRepository.GetTagAsync();
            Assert.Equal(_mockData.Tags.Count, tags.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetTagAsync_IdIsInRange_ReturnTag(int tagId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var tag = await articleServices.TagRepository.GetTagAsync(tagId);
            Assert.Equal(tagId, tag.Id);
            Assert.Equal(_mockData.Tags[tagId - 1].Content, tag.Content);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task GetTagAsync_IdIsOutOfRange_ExceptionThrow(int tagId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.TagRepository.GetTagAsync(tagId));
        }

        [Fact]
        public async Task GetTagCountAsync_ReturnTagsCount()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var count = await articleServices.TagRepository.GetTagsCountAsync();
            Assert.Equal(_mockData.Tags.Count, count);
        }

        [Fact]
        public async Task AddTag_TagIsValid_TagIsRestoredInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var newTag = new TagAddDAO()
            {
                Content = "Tag 4"
            };
            await articleServices.TagRepository.AddTagAsync(newTag);
            await articleServices.SaveAsync();

            var tag = await articleServices.TagRepository.GetTagAsync(4);
            Assert.Equal("Tag 4", tag.Content);
        }

        [Fact]
        public async Task AddTag_TagIsNull_ExceptionThrow()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<ArgumentNullException>(() => articleServices.TagRepository.AddTagAsync(null));
        }

        [Fact]
        public async Task UpdateTag_TagChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var updateTag = new TagUpdateDAO()
            {
                Id = 1,
                Content = "Changed Tag 1",
            };
            await articleServices.TagRepository.UpdateTagAsync(updateTag);
            await articleServices.SaveAsync();

            var newTag = await articleServices.TagRepository.GetTagAsync(1);
            Assert.Equal("Changed Tag 1", newTag.Content);
        }

        [Fact]
        public async Task DeleteTag_IdIsInRange_RemoveTagAndArticleTagsInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.TagRepository.DeleteTagAsync(1);
            await articleServices.SaveAsync();

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.TagRepository.GetTagAsync(1));

            // whether the article that has this tag really delete this tag in database
            var article1 = await articleServices.ArticleRepository.GetArticleAsync(1);
            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            var article3 = await articleServices.ArticleRepository.GetArticleAsync(3);
            Assert.Equal(2, article1.Tags.Count);
            Assert.Equal(2, article2.Tags.Count);
            Assert.Single(article3.Tags);

            Assert.Equal(5, context.ArticleTags.Count());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task DeleteTag_IdIsOutOfRange_ExceptionThrow(int tagId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.TagRepository.DeleteTagAsync(tagId));
        }

        // Union Test
        [Fact]
        public async Task AddTagForArticle_AddNewTagForValidArticle_ArticleHasNewTag()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.AddTagForArticleAsync(2, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(3, article.Tags.Count);
        }

        [Fact]
        public async Task AddTagForArticle_AddExistentTagForValidArticle_NothingChanged()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.AddTagForArticleAsync(2, 2);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(2, article.Tags.Count);
        }

        [Theory]
        [InlineData(2, -1)]
        [InlineData(2, 0)]
        [InlineData(2, 4)]
        [InlineData(-1, 2)]
        [InlineData(0, 2)]
        [InlineData(4, 2)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(4, 4)]
        public async Task AddTagForArticle_ArticleIdOrTagIdIsOutOfRange_ExceptionThrow(
            int articleId, int tagId )
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.AddTagForArticleAsync(articleId, tagId)
            );
        }

        [Fact]
        public async Task DeleteTagFromArticle_ValidId_DeleteTagFromArticleInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.DeleteTagFromArticleAsync(1, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(2, article.Tags.Count);
        }

        [Fact]
        public async Task DeleteTagFromArticle_ArticleDontHaveTheTag_NothingChanged()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.DeleteTagFromArticleAsync(3, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(3, article.Tags.Count);
        }

        [Theory]
        [InlineData(2, -1)]
        [InlineData(2, 0)]
        [InlineData(2, 4)]
        [InlineData(-1, 2)]
        [InlineData(0, 2)]
        [InlineData(4, 2)]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        [InlineData(4, 4)]
        public async Task DeleteTagFromArticle_ArticleIdOrTagIdIsOutOfRange_ExceptionThrow(
            int articleId, int tagId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.DeleteTagFromArticleAsync(articleId, tagId)
            );
        }

        [Fact]
        public async Task GetTagsByQueryAsync_TitleIsInData_ReturnArticle()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDAO = new TagQueryDAO()
            {
                Content = "Tag 1",
            };
            var tags = await articleServices.TagRepository.GetTagsByQueryAsync(queryDAO);
            Assert.Single(tags);
            Assert.Equal("Tag 1", tags[0].Content);
        }

        [Theory]
        [InlineData("nonexistent title")]
        [InlineData("Tag 1 ")]
        public async Task GetArticleInfoByQueryAsync_TitleIsNotInData_ReturnNull(string content)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDAO = new TagQueryDAO()
            {
                Content = content,
            };
            var tags = await articleServices.TagRepository.GetTagsByQueryAsync(queryDAO);
            Assert.Empty(tags);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task GetArticleInfoByQueryAsync_TitleIsNullOrWhiteSpace_IgnoreTitleFilter(string content)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var queryDAO = new TagQueryDAO()
            {
                Content = content,
            };
            var tags = await articleServices.TagRepository.GetTagsByQueryAsync(queryDAO);
            Assert.Equal(3, tags.Count);
        }

        //TODO test query string in pagination
    }
}
