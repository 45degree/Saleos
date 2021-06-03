using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DTO;
using Saleos.Entity;
using Saleos.Entity.Data;
using Saleos.Entity.Services;
using Xunit;

namespace Saleos.Test.Entity.Test
{
    public abstract class ArticleRepositoryTest
    {
        private DbContextOptions<HomePageDbContext> ContextOptions { get; }
        protected ArticleRepositoryTest(DbContextOptions<HomePageDbContext> contextOptions)
        {
            this.ContextOptions = contextOptions;
            Seed();
        }

        private void Seed()
        {
            using var context = new HomePageDbContext(ContextOptions);
            EntityTest.SeedDate(context);
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
            Assert.Equal(3, article1.ArticleTags.Count);

            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(2, article2.Id);
            Assert.Equal("Title 2", article2.Title);
            Assert.Equal("Content 2", article2.Content);
            Assert.Equal("Abstract 2", article2.Abstract);
            Assert.Equal(2, article2.ArticleTags.Count);
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
        public async Task GetAllArticleInfo_GetAllArticleInfo()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);

            var articles = await articleServices.ArticleInfoRepository.GetAllArticleInfo();
            Assert.Equal(3, articles.Count);
            Assert.Equal(3, articles[0].Tags.Count);
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
            var newArticle = new Article()
            {
                Title = "Title 4",
                Abstract = "Abstract 4",
                Content = "Content 4",
                ImageUrl = "ImgUrl 4",
                CreateTime = new DateTime(2020, 2, 3),
                LastModifiedTime = DateTime.Now
            };
            articleServices.ArticleRepository.AddArticle(newArticle);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(4);
            Assert.Equal(4, article.Id);
            Assert.Equal("Title 4", article.Title);
            Assert.Equal("Abstract 4", article.Abstract);
            Assert.Equal("ImgUrl 4", article.ImageUrl);
            Assert.Equal("Content 4", article.Content);
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
        public async void UpdateArticle_ArticleChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            article.Content = "Changed Content 1";
            articleServices.ArticleRepository.UpdateArticle(article);
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task TagIsExisted_IdIsInRange_ReturnTrue(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.True(await articleServices.TagRepository.TagIsExistedAsync(articleId));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(4)]
        public async Task TagIsExisted_IdIsOutOfRange_ReturnFalse(int articleId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.False(await articleServices.TagRepository.TagIsExistedAsync(articleId));
        }

        [Fact]
        public async void GetTagAsync_ReturnAllTags()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var tags= await articleServices.TagRepository.GetTagAsync();
            Assert.Equal(3, tags.Count);

            foreach (var tag in tags)
            {
                switch (tag.Id)
                {
                    case 1:
                        Assert.Equal("Tag 1", tag.Content);
                        Assert.Single(tag.ArticleTag);
                        break;
                    case 2:
                        Assert.Equal("Tag 2", tag.Content);
                        Assert.Equal(2, tag.ArticleTag.Count);
                        break;
                    case 3:
                        Assert.Equal("Tag 3", tag.Content);
                        Assert.Equal(3, tag.ArticleTag.Count);
                        break;
                    default:
                        Assert.True(false);
                        break;
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async void GetTagAsync_IdIsInRange_ReturnTag(int tagId)
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var tag = await articleServices.TagRepository.GetTagAsync(tagId);
            Assert.Equal(tagId, tag.Id);
            Assert.Equal($"Tag {tagId}", tag.Content);
            Assert.Equal(tagId, tag.ArticleTag.Count);  // tagId 和 tag 对应的文章数是相同的
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
        public async void GetTagCountAsync_ReturnTagsCount()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var count = await articleServices.TagRepository.GetTagsCountAsync();
            Assert.Equal(3, count);
        }

        [Fact]
        public async void AddTag_TagIsValid_TagIsRestoredInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var newTag = new Tag()
            {
                Content = "Tag 4"
            };
            articleServices.TagRepository.AddTag(newTag);
            await articleServices.SaveAsync();

            var tag = await articleServices.TagRepository.GetTagAsync(4);
            Assert.Equal("Tag 4", tag.Content);
        }

        [Fact]
        public async void AddTag_TagIsNull_ExceptionThrow()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            Assert.Throws<ArgumentNullException>(() => articleServices.TagRepository.AddTag(null));
        }

        [Fact]
        public async void UpdateTag_TagChangedInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            var tag = await articleServices.TagRepository.GetTagAsync(1);
            tag.Content = "Changed Tag 1";
            articleServices.TagRepository.UpdateTag(tag);
            await articleServices.SaveAsync();

            var newTag = await articleServices.TagRepository.GetTagAsync(1);
            Assert.Equal("Changed Tag 1", newTag.Content);
        }

        [Fact]
        public async void DeleteTag_IdIsInRange_RemoveTagAndArticleTagsInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.TagRepository.DeleteTagAsync(1);
            await articleServices.SaveAsync();

            await Assert.ThrowsAsync<IndexOutOfRangeException>(() =>
                articleServices.TagRepository.GetTagAsync(1));

            // whether the article which has this tag in database delete this tag
            var article1 = await articleServices.ArticleRepository.GetArticleAsync(1);
            var article2 = await articleServices.ArticleRepository.GetArticleAsync(2);
            var article3 = await articleServices.ArticleRepository.GetArticleAsync(3);
            Assert.Equal(2, article1.ArticleTags.Count);
            Assert.Equal(2, article2.ArticleTags.Count);
            Assert.Single(article3.ArticleTags);

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
            Assert.Equal(3, article.ArticleTags.Count);
        }

        [Fact]
        public async Task AddTagForArticle_AddExistentTagForValidArticle_NothingChanged()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.AddTagForArticleAsync(2, 2);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(2);
            Assert.Equal(2, article.ArticleTags.Count);
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
        public async void DeleteTagFromArticle_ValidId_DeleteTagFromArticleInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.DeleteTagFromArticleAsync(1, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(2, article.ArticleTags.Count);
        }

        [Fact]
        public async void DeleteTagFromArticle_ArticleDontHaveTheTag_NothingChanged()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.DeleteTagFromArticleAsync(3, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(3, article.ArticleTags.Count);
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
    }
}