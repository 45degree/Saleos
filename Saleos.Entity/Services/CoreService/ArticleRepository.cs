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

        public async Task<ArticleDAO> GetArticleAsync(int articleId)
        {
            if (articleId <= 0)
                throw new IndexOutOfRangeException($"{articleId} is out of range");

            return await _homePageDbContext.Article
                .Where(x => x.Id == articleId)
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Select(x => x.GetArticleDAOFromArticle())
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetArticleCountAsync()
        {
            return await _homePageDbContext.Article.CountAsync();
        }

        public async Task AddArticleAsync(ArticleAddDAO article)
        {
            if (article == null) throw new ArgumentNullException($"{nameof(article)} is null");
            await _homePageDbContext.Article
                .AddAsync(await article.GetArticleFromArticleAddDAO(_homePageDbContext));
        }

        public async Task UpdateArticleAsync(ArticleUpdateDAO articleUpdate)
        {
            if (articleUpdate == null)
                throw new ArgumentNullException($"{nameof(articleUpdate)} is null");

            var article = await _homePageDbContext.Article
                .SingleOrDefaultAsync(x => x.Id == articleUpdate.Id);
            article.LastModifiedTime = articleUpdate.LastModifiedTime;

            if (articleUpdate.Content != null) article.Content = articleUpdate.Content;
            if (articleUpdate.Title != null) article.Title = articleUpdate.Title;
            article.IsReprint = articleUpdate.IsReprint;
            if (articleUpdate.IsReprint) article.ReprintUri = articleUpdate.ReprintUri;
            if (articleUpdate.CategoryId > 0)
            {
                var category = await _homePageDbContext.Categories
                    .SingleOrDefaultAsync(x => x.Id == articleUpdate.CategoryId);
                article.Category = category;
            }
            if (articleUpdate.Tags != null)
            {
                await _homePageDbContext.ArticleTags.Where(x => x.ArticleId == articleUpdate.Id)
                    .ForEachAsync( x=> _homePageDbContext.Remove(x));
                article.ArticleTags = new List<ArticleTag>();
                foreach (var tagId in articleUpdate.Tags)
                {
                    await _homePageDbContext.ArticleTags.AddAsync(new ArticleTag()
                    {
                        ArticleId = articleUpdate.Id,
                        TagId = tagId
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
