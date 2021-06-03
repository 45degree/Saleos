using System;
using System.Collections.Generic;

namespace Saleos.DTO
{
    public class ArticleInfoDto
    {
        public int Id { get; set; }
        public string Abstract { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public List<TagDto> Tags { get; set; } 
    }
}