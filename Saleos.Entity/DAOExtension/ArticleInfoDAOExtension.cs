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
using Saleos.DAO;

namespace Saleos.Entity.DAOExtension
{
    public static class ArticleInfoDAOExtension
    {
        public static ArticleInfoDAO GetArticleInfoDAOFromArticle(this Article article)
        {
            var articleInfoDAO = new ArticleInfoDAO()
            {
                Id = article.Id,
                Abstract = article.Abstract,
                Title = article.Title,
                ImgUrl = article.ImageUrl,
                CreateTime = article.CreateTime,
                LastModifiedTime = article.LastModifiedTime,
                Tags = new List<TagDAO>(),
            };
            foreach (var articleTag in article.ArticleTags)
            {
                articleInfoDAO.Tags.Add(articleTag.Tag.GetTagDAOFromTag());
            }

            return articleInfoDAO;
        }
    }
}
