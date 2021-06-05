using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saleos.Entity
{
    public class Article
    {
        public Article()
        {
            ArticleTags = new List<ArticleTag>();
        }

        [Required]
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        /// <summary>
        /// whether this article is a reprint
        /// </summary>
        public bool IsReprint { get; set; } = false;

        /// <summary>
        /// the reprint Url if this article is a reprint
        /// </summary>
        public string OriginalArticleUri {get; set;}

        /// <summary>
        /// the article's title
        /// </summary>
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        /// <summary>
        /// the article's content
        /// </summary>
        public string Content {get; set;}

        /// <summary>
        /// the article's abstract
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// the tags that this article contains
        /// </summary>
        public List<ArticleTag> ArticleTags { get; set; }
        
        /// <summary>
        /// the category that the article belongs to
        /// </summary>
        public Category Category { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsReprint && string.IsNullOrEmpty(OriginalArticleUri)) yield return new ValidationResult("需要提供原文章uri地址");
        }
    }

    /// <summary>
    /// a join table that connect Articles and Tags
    /// </summary>
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }
        public Article Article { get; set; }
        public Tag Tag { get; set; }
    }
}