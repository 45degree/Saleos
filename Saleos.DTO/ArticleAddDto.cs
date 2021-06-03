using System;
using System.ComponentModel.DataAnnotations;

namespace Saleos.DTO
{
    public class ArticleAddDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreateTime { get; set; } 
    }
}