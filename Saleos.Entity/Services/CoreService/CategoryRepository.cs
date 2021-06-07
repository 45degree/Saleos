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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly HomePageDbContext _homePageDbContext;
        
        public CategoryRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }
        
        public async Task<bool> CategoryIsExistAsync(int categoryId)
        {
            return await _homePageDbContext.Categories.AnyAsync(x => x.Id == categoryId);
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _homePageDbContext.Categories.CountAsync();
        }

        public async Task<List<CategoryDto>> GetCategoryAsync()
        {
            return await _homePageDbContext.Categories
                .Select(x => x.GetCategoryDtoFromCategory())
                .ToListAsync();
        }

        public async Task<CategoryDto> GetCategoryAsync(int categoryId)
        {
            if(!await CategoryIsExistAsync(categoryId))
                throw new IndexOutOfRangeException($"{nameof(categoryId)} is out of range");
            
            return await _homePageDbContext.Categories
                .Where(x => x.Id == categoryId)
                .Select(x => x.GetCategoryDtoFromCategory())
                .FirstOrDefaultAsync();
        }

        public async Task<List<CategoryDto>> GetCategoryByQueryAsync(CategoryQueryDto categoryQueryDto)
        {
            var queryString = _homePageDbContext.Categories as IQueryable<Category>;

            // Filter Title
            if (!string.IsNullOrWhiteSpace(categoryQueryDto.Content))
            {
                queryString = queryString.Where(x => categoryQueryDto.Content.Equals(x.Content));
            }
            
            return await queryString.OrderBy(x => x.Id)
                .Skip(categoryQueryDto.PageSize * (categoryQueryDto.PageNumber - 1))
                .Take(categoryQueryDto.PageSize)
                .Select(x => x.GetCategoryDtoFromCategory())
                .ToListAsync();
        }

        public async Task AddCategoryAsync(CategoryAddDto categoryAddDto)
        {
            if (categoryAddDto == null) throw new ArgumentNullException($"{nameof(categoryAddDto)} is null");
            await _homePageDbContext.Categories.AddAsync(categoryAddDto.GetCategoryFromCateGoryAddDto());
        }

        public async Task UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            var category = await _homePageDbContext.Categories.SingleOrDefaultAsync(x => x.Id == categoryUpdateDto.Id);
            category.Content = categoryUpdateDto.Content;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            if (!await CategoryIsExistAsync(categoryId))
                throw new IndexOutOfRangeException($"{nameof(categoryId)} is out of range");

            var category = await _homePageDbContext.Categories
                .Where(x => x.Id == categoryId)
                .SingleOrDefaultAsync();

            await _homePageDbContext.Article
                .Where(x => x.Category == category)
                .ForEachAsync(x => x.Category = null);

            _homePageDbContext.Categories.Remove(category);
        }
    }
}