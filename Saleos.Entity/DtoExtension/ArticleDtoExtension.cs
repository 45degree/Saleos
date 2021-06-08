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
using Saleos.DTO;
using Saleos.Entity.Data;

namespace Saleos.Entity.DtoExtension
{
    public static class ArticleDtoExtension
    {
       public static async Task<Article> GetArticleFromArticleAddDto(this ArticleAddDto addDto, HomePageDbContext context)
       {
            var category = await context.Categories.SingleOrDefaultAsync(x => x.Id == addDto.CategoryId);
            var article = new Article
            {
                Content = addDto.Content,
                Title = addDto.Title,
                Category = category,
                CreateTime = addDto.CreateTime,
                LastModifiedTime = addDto.CreateTime
            };
            return article;
        }

        public static ArticleDto GetArticleDtoFromArticle(this Article article)
        {
            var articleDto =  new ArticleDto()
            {
                Id = article.Id,
                Title = article.Title,
                Abstract = article.Abstract,
                ImgUrl = article.ImageUrl,
                Category = article.Category.GetCategoryDtoFromCategory(),
                Content = article.Content,
                CreateTime = article.CreateTime,
                LastModifiedTime = article.LastModifiedTime,
                Tags = new List<TagDto>(),
            };
            foreach (var articleTag in article.ArticleTags)
            {
                articleDto.Tags.Add(articleTag.Tag.GetTagDtoFromTag());
            }
            return articleDto;
        }
    }
}