using System;
using System.Threading.Tasks;

namespace Saleos.Entity.Services
{
    public interface IArticleRepository
    {
        /// <summary>
        /// 通过Id判断文章是否存在
        /// </summary>
        /// <param name="articleId">文章的Id</param>
        /// <returns>
        /// 如果存在该文章，则返回true，否则返回false
        /// </returns>
        public Task<bool> ArticleIsExisted(int articleId);

        /// <summary>
        /// 通过文章id异步获取文章
        /// </summary>
        /// <param name="articleId">文章的Id</param>
        /// <returns>
        /// 如果存在该文章，则返回该文章
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// 当articleId没有对应的文章时抛出该异常，例如articleId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public Task<Article> GetArticleAsync(int articleId);

        /// <summary>
        /// 获取文章的数量
        /// </summary>
        /// <returns>文章数量</returns>
        public Task<int> GetArticleCountAsync();

        /// <summary>
        /// 添加一篇文章
        /// </summary>
        /// <param name="article">需添加的文章</param>
        /// <exception cref="ArgumentNullException">
        /// 当article为null时抛出异常
        /// </exception>
        public void AddArticle(Article article);

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="article">修改后的文章</param>
        public void UpdateArticle(Article article);

        /// <summary>
        /// delete an article
        /// </summary>
        /// <param name="articleId">文章的Id</param>
        /// <exception cref="IndexOutOfRangeException">
        /// 当articleId没有对应的文章时抛出该异常，例如articleId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public Task DeleteArticleAsync(int articleId);
    }
}