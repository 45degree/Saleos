using System.Collections.Generic;
using Saleos.DTO;

namespace Saleos.Entity
{
    public static class DtoExtension
    {
       public static Article GetArticle(this ArticleAddDto addDto)
        {
            var article = new Article
            {
                Content = addDto.Content,
                Title = addDto.Title,
                CreateTime = addDto.CreateTime,
                LastModifiedTime = addDto.CreateTime
            };
            return article;
        }

        public static ArticleDto GetArticleDto(this Article article)
        {
            var articleDto =  new ArticleDto()
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreateTime = article.CreateTime,
                LastModifiedTime = article.LastModifiedTime,
                Tags = new List<TagDto>(),
            };
            foreach (var articleTag in article.ArticleTags)
            {
                articleDto.Tags.Add(articleTag.Tag.GetTagDto());
            }
            return articleDto;
        }


        public static TagDto GetTagDto(this Tag tag)
        {
            var tagDto = new TagDto()
            {
                Id = tag.Id,
                Tags = tag.Content,
            };
            return tagDto;
        }

        public static Tag GetTag(this TagAddDto addDto)
        {
            var tag = new Tag() { Content = addDto.Content };
            return tag;
        }

        public static ArticleInfoDto GetArticleInfoDto(this Article article)
        {
            var articleInfoDto = new ArticleInfoDto()
            {
                Id = article.Id,
                Abstract = article.Abstract,
                Title = article.Title,
                ImgUrl = article.ImageUrl,
                CreateTime = article.CreateTime,
                LastModifiedTime = article.LastModifiedTime,
                Tags = new List<TagDto>(),
            };
            foreach (var articleTag in article.ArticleTags)
            {
                articleInfoDto.Tags.Add(articleTag.Tag.GetTagDto());
            }

            return articleInfoDto;
        } 
    }
}