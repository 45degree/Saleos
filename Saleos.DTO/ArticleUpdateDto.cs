using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saleos.DTO
{
    public class ArticleUpdateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime LastModifiedTime { get; set; }

        [Required]
        public List<int> Tags { get; set; } 
    }
}