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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DAO;
using Saleos.Entity.Data;
using Saleos.Entity.DAOExtension;

namespace Saleos.Entity.Services.CoreServices
{
    public class ArticleInfoRepository : IArticleInfoRepository
    {
        private readonly HomePageDbContext _homePageDbContext;

        public ArticleInfoRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }

        public async Task<List<ArticleInfoDAO>> GetArticleInfoByQueryAsync(ArticlesQueryDAO query)
        {
            if (query == null)
                throw new ArgumentNullException($"{nameof(query)} 为null或则全是空格");

            var queryString = _homePageDbContext.Article as IQueryable<Article>;

            // Filter Title
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                queryString = queryString.Where(x => query.Title.Equals(x.Title));
            }

            // whether is overflow
            if(int.MaxValue / query.PageSize < query.PageNumber)
            {
                return new List<ArticleInfoDAO>();
            }

            queryString = queryString.OrderBy(x => x.Id)
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize);

            queryString = queryString.Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag);

            return await queryString.Select(x => x.GetArticleInfoDAOFromArticle()).ToListAsync();
        }

        public async Task<List<ArticleInfoDAO>> GetAllArticleInfo()
        {
            return await _homePageDbContext.Article
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Select(x => x.GetArticleInfoDAOFromArticle())
                .ToListAsync();
        }

        public async Task<ArticleInfoDAO> GetArticleInfo(int articleId)
        {
            return await _homePageDbContext.Article
                .Where(x => x.Id == articleId)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Select(x => x.GetArticleInfoDAOFromArticle())
                .SingleOrDefaultAsync();
        }
    }
}
