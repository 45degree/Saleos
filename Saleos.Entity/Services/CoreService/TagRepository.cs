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

        public async Task<List<TagDAO>> GetTagAsync()
        {
            return await _homePageDbContext.Tags
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .Select(x => x.GetTagDAOFromTag())
                .ToListAsync();
        }

        public async Task<TagDAO> GetTagAsync(int tagId)
        {
            if (!await TagIsExistAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            return await _homePageDbContext.Tags
                .Where(x => x.Id == tagId)
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .Select(x => x.GetTagDAOFromTag())
                .FirstOrDefaultAsync();
        }

        public async Task<List<TagDAO>> GetTagsByQueryAsync(TagQueryDAO tagQueryDAO)
        {
            var queryString = _homePageDbContext.Tags as IQueryable<Tag>;

            // Filter Title
            if (!string.IsNullOrWhiteSpace(tagQueryDAO.Content))
            {
                queryString = queryString.Where(x => tagQueryDAO.Content.Equals(x.Content));
            }

            return await queryString.OrderBy(x => x.Id)
                .Skip(tagQueryDAO.PageSize * (tagQueryDAO.PageNumber - 1))
                .Take(tagQueryDAO.PageSize)
                .Select(x => x.GetTagDAOFromTag())
                .ToListAsync();
        }

        public async Task AddTagAsync(TagAddDAO tagAddDAO)
        {
            if (tagAddDAO == null) throw new ArgumentNullException($"{nameof(tagAddDAO)} 为空");
            await _homePageDbContext.Tags.AddAsync(tagAddDAO.GetTagFromTagAddDAO());
        }

        public async Task UpdateTagAsync(TagUpdateDAO tagUpdateDAO)
        {
            var tag = await _homePageDbContext.Tags.SingleOrDefaultAsync(x => x.Id == tagUpdateDAO.Id);
            tag.Content = tagUpdateDAO.Content;
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
