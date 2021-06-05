using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.Entity;
using Saleos.Entity.Data;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Entity.Test
{
    public abstract class TagRepositoryTest : BaseServicesTest
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
        public async void DeleteTagFromArticle_ValidId_DeleteTagFromArticleInData()
        {
            await using var context = new HomePageDbContext(ContextOptions);
            ArticleServices articleServices = new ArticleServicesImpl(context);
            await articleServices.DeleteTagFromArticleAsync(1, 1);
            await articleServices.SaveAsync();

            var article = await articleServices.ArticleRepository.GetArticleAsync(1);
            Assert.Equal(2, article.Tags.Count);
        }

        [Fact]
        public async void DeleteTagFromArticle_ArticleDontHaveTheTag_NothingChanged()
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
    }
}