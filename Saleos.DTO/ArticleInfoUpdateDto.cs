using System.ComponentModel.DataAnnotations;

namespace Saleos.DTO
{
    public class ArticleInfoUpdateDto
    {
        [Required]
        public string Title { get; set; }

        public string ImgUrl { get; set; }

        public string Abstract { get; set; } 
    }
}