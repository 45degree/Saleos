using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DTO;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services.CoreServices
{
    public class ArticleInfoRepository : IArticleInfoRepository
    {
        private readonly HomePageDbContext _homePageDbContext;

        public ArticleInfoRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }

        public async Task<List<ArticleInfoDto>> GetArticleInfoByQueryAsync(ArticlesQueryDto query)
        {
            if (query == null)
                throw new ArgumentNullException($"{nameof(query)} 为null或则全是空格");

            var queryString = _homePageDbContext.Article as IQueryable<Article>;

            // Filter Title
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                queryString = queryString.Where(x => query.Title.Equals(x.Title));
            }

            queryString = queryString.Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .OrderBy(x => x.Id);

            queryString = queryString.Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag);

            return await queryString.Select(x => x.GetArticleInfoDto()).ToListAsync();
        }

        public async Task<List<ArticleInfoDto>> GetAllArticleInfo()
        {
            return await _homePageDbContext.Article
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Select(x => x.GetArticleInfoDto())
                .ToListAsync();
        }
    } 
}