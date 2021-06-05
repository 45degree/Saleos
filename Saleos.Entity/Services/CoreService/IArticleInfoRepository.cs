using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saleos.DTO;

namespace Saleos.Entity.Services.CoreServices
{
    public interface IArticleInfoRepository
    {
        /// <summary>
        /// 通过查询获取文章
        /// </summary>
        /// <param name="query">query Information</param>
        /// <returns>拥有该标题的Title</returns>
        /// <exception cref="ArgumentNullException">
        /// query为null时抛出该异常
        /// </exception>
        public Task<List<ArticleInfoDto>> GetArticleInfoByQueryAsync(ArticlesQueryDto query);

        /// <summary>
        /// get all the article's information in database
        /// </summary>
        /// <returns> the collection of the article's information </returns>
        public Task<List<ArticleInfoDto>> GetAllArticleInfo(); 
    }
}