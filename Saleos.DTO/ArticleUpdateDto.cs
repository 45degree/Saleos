using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saleos.DTO
{
    public class ArticleUpdateDto
    {
        [Required]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public DateTime LastModifiedTime { get; set; }

        public List<int> Tags { get; set; } = null;
    }
}