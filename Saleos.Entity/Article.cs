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
        /// 是否是转载文章
        /// </summary>
        [Required]
        public bool IsReprint { get; set; }

        /// <summary>
        /// 转载文案的地址
        /// </summary>
        public string OriginalArticleUri {get; set;}

        /// <summary>
        /// 文章标题
        /// </summary>
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content {get; set;}

        /// <summary>
        /// 文章摘要
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// 文章包含的标签
        /// </summary>
        public List<ArticleTag> ArticleTags { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsReprint && string.IsNullOrEmpty(OriginalArticleUri)) yield return new ValidationResult("需要提供原文章uri地址");
        }
    }

    /// <summary>
    /// Article 和 tag 的关联表
    /// </summary>
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public int TagId { get; set; }
        public Article Article { get; set; }
        public Tag Tag { get; set; }
    }
}