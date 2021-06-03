using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly HomePageDbContext _homePageDbContext;

        public ArticleRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }

        public async Task<bool> ArticleIsExisted(int articleId)
        {
            return await _homePageDbContext.Article.AnyAsync(x => x.Id == articleId);
        }

        public async Task<Article> GetArticleAsync(int articleId)
        {
            if (articleId <= 0)
                throw new IndexOutOfRangeException("articleId 超出索引范围");

            return await _homePageDbContext.Article
                .Where(x => x.Id == articleId)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetArticleCountAsync()
        {
            return await _homePageDbContext.Article.CountAsync();
        }

        public void AddArticle(Article article)
        {
            if (article == null) throw new ArgumentNullException($"{nameof(article)} 为null");
            _homePageDbContext.Article.Add(article);
        }

        public void UpdateArticle(Article article)
        {
            // EF 自动最终, 不需要实现
        }

        public async Task DeleteArticleAsync(int articleId)
        {
            if (!await ArticleIsExisted(articleId))
                throw new IndexOutOfRangeException("articleId 超出索引范围");

            var article = await _homePageDbContext.Article
                .Where(x => x.Id == articleId)
                .Include(x => x.ArticleTags)
                .FirstOrDefaultAsync();

            foreach (var articleTag in article.ArticleTags)
            {
                _homePageDbContext.Remove(articleTag);
            }

            _homePageDbContext.Article.Remove(article);
        }
    }
}
