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
using Saleos.DTO;
using Saleos.Entity.Data;
using Saleos.Entity.DtoExtension;

namespace Saleos.Entity.Services.CoreServices
{
    public class TagRepository : ITagRepository
    {
       private readonly HomePageDbContext _homePageDbContext;

        public TagRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }

        public async Task<bool> TagIsExistAsync(int tagId)
        {
            return await _homePageDbContext.Tags.AnyAsync(x => x.Id == tagId);
        }

        public async Task<int> GetTagsCountAsync()
        {
            return await _homePageDbContext.Tags.CountAsync();
        }

        public async Task<List<TagDto>> GetTagAsync()
        {
            return await _homePageDbContext.Tags
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .Select(x => x.GetTagDtoFromTag())
                .ToListAsync();
        }

        public async Task<TagDto> GetTagAsync(int tagId)
        {
            if (!await TagIsExistAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            return await _homePageDbContext.Tags
                .Where(x => x.Id == tagId)
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .Select(x => x.GetTagDtoFromTag())
                .FirstOrDefaultAsync();
        }

        public async Task<List<TagDto>> GetTagsByQueryAsync(TagQueryDto tagQueryDto)
        {
            var queryString = _homePageDbContext.Tags as IQueryable<Tag>;

            // Filter Title
            if (!string.IsNullOrWhiteSpace(tagQueryDto.Content))
            {
                queryString = queryString.Where(x => tagQueryDto.Content.Equals(x.Content));
            }
            
            return await queryString.OrderBy(x => x.Id)
                .Skip(tagQueryDto.PageSize * (tagQueryDto.PageNumber - 1))
                .Take(tagQueryDto.PageSize)
                .Select(x => x.GetTagDtoFromTag())
                .ToListAsync();
        }

        public async Task AddTagAsync(TagAddDto tagAddDto)
        {
            if (tagAddDto == null) throw new ArgumentNullException($"{nameof(tagAddDto)} 为空");
            await _homePageDbContext.Tags.AddAsync(tagAddDto.GetTagFromTagAddDto());
        }

        public async Task UpdateTagAsync(TagUpdateDto tagUpdateDto)
        {
            var tag = await _homePageDbContext.Tags.SingleOrDefaultAsync(x => x.Id == tagUpdateDto.Id);
            tag.Content = tagUpdateDto.Content;
        }

        public async Task DeleteTagAsync(int tagId)
        {
            if (!await TagIsExistAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            var tag = await _homePageDbContext.Tags
                .Where(x => x.Id == tagId)
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .SingleOrDefaultAsync();

            foreach (var articleTag in tag.ArticleTag)
            {
                _homePageDbContext.Remove(articleTag);
            }

            _homePageDbContext.Remove(tag);
        } 
    }
}