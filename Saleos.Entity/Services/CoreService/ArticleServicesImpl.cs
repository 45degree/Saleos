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
            CategoryRepository = new CategoryRepository(homePageDbContext);
        }

        public override async Task AddTagForArticleAsync(int articleId, int tagId)
        {
            if (!await ArticleRepository.ArticleIsExisted(articleId))
                throw new IndexOutOfRangeException("articleId 超出索引范围");
            if (!await TagRepository.TagIsExistAsync(tagId))
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
            if (!await TagRepository.TagIsExistAsync(tagId))
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