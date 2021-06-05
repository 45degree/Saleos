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
            var queryDto = new ArticlesQueryDto()
            {
                Title = "Title 1",
            };
            var article = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Single(article);
            Assert.Equal(1, article[0].Id);
            Assert.Equal("Abstract 1", article[0].Abstract);
            Assert.Equal(3, article[0].Tags.Count);
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
            var article = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
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
            var articles = await articleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(queryDto);
            Assert.Equal(3, articles.Count);
        }
        
        [Fact]
        public async Task GetAllArticleInfo_GetAllArticleInfo()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);

            var articles = await articleServices.ArticleInfoRepository.GetAllArticleInfo();
            Assert.Equal(3, articles.Count);
            Assert.Equal(3, articles[0].Tags.Count);
        }
    }
}