using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services.CoreServices
{
    public class ArticleServicesImpl : ArticleServices
    {
        private readonly HomePageDbContext _homePageDbContext;

        public ArticleServicesImpl(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
            ArticleInfoRepository = new ArticleInfoRepository(homePageDbContext);
            TagRepository = new TagRepository(homePageDbContext);
            ArticleRepository = new ArticleRepository(homePageDbContext);
        }

        public override async Task AddTagForArticleAsync(int articleId, int tagId)
        {
            if (!await ArticleRepository.ArticleIsExisted(articleId))
                throw new IndexOutOfRangeException("articleId 超出索引范围");
            if (!await TagRepository.TagIsExistedAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            var articleTag = new ArticleTag()
            {
                ArticleId = articleId,
                TagId = tagId
            };

            if (! await _homePageDbContext.ArticleTags.AnyAsync(x => x.ArticleId == articleId && x.TagId == tagId))
            {
                await _homePageDbContext.ArticleTags.AddAsync(articleTag);
            }
        }

        public override async Task DeleteTagFromArticleAsync(int articleId, int tagId)
        {
            if (!await ArticleRepository.ArticleIsExisted(articleId))
                throw new IndexOutOfRangeException("articleId 超出索引范围");
            if (!await TagRepository.TagIsExistedAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            if(! await _homePageDbContext.ArticleTags.AnyAsync(x => x.ArticleId == articleId && x.TagId == tagId)) return;
            var articleTag = new ArticleTag()
            {
                TagId = tagId,
                ArticleId = articleId
            };
            _homePageDbContext.Remove(articleTag);
        }

        public override async Task<bool> SaveAsync()
        {
            return await _homePageDbContext.SaveChangesAsync() >= 0;
        } 
    }
}