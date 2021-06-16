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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DAO;
using Saleos.Entity.Data;

namespace Saleos.Entity.DAOExtension
{
    public static class ArticleDAOExtension
    {
       public static async Task<Article> GetArticleFromArticleAddDAO(this ArticleAddDAO addDAO, HomePageDbContext context)
       {
            var category = await context.Categories.SingleOrDefaultAsync(x => x.Id == addDAO.CategoryId);
            var article = new Article
            {
                Content = addDAO.Content,
                Title = addDAO.Title,
                Category = category,
                CreateTime = addDAO.CreateTime,
                LastModifiedTime = addDAO.CreateTime,
                IsReprint = addDAO.IsReprint,
                ReprintUri = addDAO.ReprintUri
            };
            return article;
        }

        public static ArticleDAO GetArticleDAOFromArticle(this Article article)
        {
            var articleDAO =  new ArticleDAO()
            {
                Id = article.Id,
                Title = article.Title,
                Abstract = article.Abstract,
                ImgUrl = article.ImageUrl,
                Category = article.Category.GetCategoryDAOFromCategory(),
                Content = article.Content,
                CreateTime = article.CreateTime,
                LastModifiedTime = article.LastModifiedTime,
                Tags = new List<TagDAO>(),
                IsReprint = article.IsReprint,
                RerpintUri = article.ReprintUri,
            };
            foreach (var articleTag in article.ArticleTags)
            {
                articleDAO.Tags.Add(articleTag.Tag.GetTagDAOFromTag());
            }
            return articleDAO;
        }
    }
}
