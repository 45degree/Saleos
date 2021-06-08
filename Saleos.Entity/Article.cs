/*
 * Copyright 2021 45degree
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        public string ReprintUri {get; set;}

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
            if (IsReprint && string.IsNullOrEmpty(ReprintUri)) yield return new ValidationResult("需要提供原文章uri地址");
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
