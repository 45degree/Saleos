using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saleos.DTO;
using Saleos.Entity.Data;

namespace Saleos.Entity.Services.CoreServices
{
    public class TagRepository : ITagRepository
    {
       private readonly HomePageDbContext _homePageDbContext;

        public TagRepository(HomePageDbContext homePageDbContext)
        {
            _homePageDbContext = homePageDbContext;
        }

        public async Task<bool> TagIsExistedAsync(int tagId)
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
                .Select(x => x.GetTagDto())
                .ToListAsync();
        }

        public async Task<Tag> GetTagAsync(int tagId)
        {
            if (!await TagIsExistedAsync(tagId))
                throw new IndexOutOfRangeException("tagId 超出索引范围");

            return await _homePageDbContext.Tags
                .Where(x => x.Id == tagId)
                .Include(x => x.ArticleTag)
                .ThenInclude(x => x.Article)
                .FirstOrDefaultAsync();
        }

        public void AddTag(Tag tag)
        {
            if (tag == null) throw new ArgumentNullException($"{nameof(tag)} 为空");
            _homePageDbContext.Tags.Add(tag);
        }

        public void UpdateTag(Tag tag)
        {
            // EF 自动最终, 不需要实现
        }

        public async Task DeleteTagAsync(int tagId)
        {
            if (!await TagIsExistedAsync(tagId))
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