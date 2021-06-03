using System;
using System.Collections.Generic;

namespace Saleos.DTO
{
    public class ArticleDto : ICloneable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();

        public object Clone()
        {
            var articleDto = new ArticleDto()
            {
                Id = Id,
                Title = Title,
                Content = Content,
                CreateTime = CreateTime,
                LastModifiedTime = LastModifiedTime,
                Tags = new List<TagDto>(),
            };

            foreach (var tagDto in Tags)
            {
                articleDto.Tags.Add(tagDto.Clone() as TagDto);
            }

            return articleDto;
        }
    }
}