using System;
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
            Assert.Equal(1, article1.Id);
            Assert.Equal("Title 1", article1.Title);
            Assert.Equal("Content 1", article1.Content);
            Assert.Equal("Abstract 1", article1.Abstract);
            Assert.Equal("Category1", article1.Category.Content);
            Assert.Equal(3, article1.Tags.Count);

            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(2, article2.Id);
            Assert.Equal("Title 2", article2.Title);
            Assert.Equal("Content 2", article2.Content);
            Assert.Equal("Abstract 2", article2.Abstract);
            Assert.Equal("Category1", article2.Category.Content);
            Assert.Equal(2, article2.Tags.Count);
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
            Assert.Equal(1, articles[0].Id);
            Assert.Equal(2, articles[1].Id);

            queryDto.PageNumber = 2;
            articles = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Single(articles);
            Assert.Equal(3, articles[0].Id);
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
                Category = new CategoryDto(){ Content = "Category4"},
                CreateTime = new DateTime(2020, 2, 3),
            };
            articleServices.ArticleRepository.AddArticle(newArticle);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(4);
            Assert.Equal(4, article.Id);
            Assert.Equal("Title 4", article.Title);
            Assert.Equal("Content 4", article.Content);
            Assert.Equal("Category4", article.Category.Content);
        }

        [Fact]
        public async Task AddArticle_ArticleIsNull_ExceptionThrow()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.Throws<ArgumentNullException>(() =>
            {
                articleServices.ArticleRepository.AddArticle(null);
            });
        }

        [Fact]
        public async Task UpdateArticle_ArticleChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var articleUpdate = new ArticleUpdateDto()
            {
                Id = 1,
                Content = "Changed Content 1",
            };
            await articleServices.ArticleRepository.UpdateArticleAsync(articleUpdate);
            await articleServices.SaveAsync();

            var newArticle = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal("Changed Content 1", newArticle.Content);
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

            var tag1 = await articleServices.TagRepository.GetTagAsync(1);
            var tag2 = await articleServices.TagRepository.GetTagAsync(2);
            var tag3 = await articleServices.TagRepository.GetTagAsync(3);
            Assert.Empty(tag1.ArticleTag);
            Assert.Single(tag2.ArticleTag);
            Assert.Equal(2, tag3.ArticleTag.Count);
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