using System;
using System.Collections.Generic;

namespace Saleos.DTO
{
    public class ArticleDto : ArticleInfoDto, ICloneable
    {
        public string Content { get; set; }
        
        public object Clone()
        {
            var articleDto = new ArticleDto()
            {
                Id = Id,
                Title = Title,
                Abstract = Abstract,
                ImgUrl = ImgUrl,
                Category = Category,
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