using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DTO;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services.CoreServices
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

        public async Task<ArticleDto> GetArticleAsync(int articleId)
        {
            if (articleId <= 0)
                throw new IndexOutOfRangeException($"{articleId} is out of range");

            return await _homePageDbContext.Article
                .Where(x => x.Id == articleId)
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Select(x => x.GetArticleDto())
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetArticleCountAsync()
        {
            return await _homePageDbContext.Article.CountAsync();
        }

        public void AddArticle(ArticleAddDto article)
        {
            if (article == null) throw new ArgumentNullException($"{nameof(article)} is null");
            _homePageDbContext.Article.Add(article.GetArticle());
        }

        public async Task UpdateArticleAsync(ArticleUpdateDto articleUpdate)
        {
            if (articleUpdate == null) throw new ArgumentNullException($"{nameof(articleUpdate)} is null");
            
            var article = await _homePageDbContext.Article.SingleOrDefaultAsync(x => x.Id == articleUpdate.Id);
            article.LastModifiedTime = articleUpdate.LastModifiedTime;
            
            if (articleUpdate.Content != null) article.Content = articleUpdate.Content;
            if (articleUpdate.Title != null) article.Title = articleUpdate.Title;
            if (articleUpdate.Tags != null)
            {
                article.ArticleTags = new List<ArticleTag>();
                foreach (var tagId in articleUpdate.Tags)
                {
                    article.ArticleTags.Add(new ArticleTag()
                    {
                        ArticleId = articleUpdate.Id,
                        TagId = tagId,
                    });
                }
            }
        }

        public async Task DeleteArticleAsync(int articleId)
        {
            if (!await ArticleIsExisted(articleId))
                throw new IndexOutOfRangeException($"{articleId} is out of range");

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
