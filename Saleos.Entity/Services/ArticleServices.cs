using System.Threading.Tasks;

namespace Saleos.Entity.Services
{
    public abstract class ArticleServices
    {
        /*  ArticleInfo */
        public IArticleInfoRepository ArticleInfoRepository { get; protected init; }

        /* tag */
        public ITagRepository TagRepository { get; protected init; }

        /* Article */
        public IArticleRepository ArticleRepository { get; protected init; }

        // union

        /// <summary>
        /// 为文章添加一个新的tag, 当文章已经存在该tag时，不会对数据库有影响
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="tagId">tag的Id</param>
        /// <exception cref="IndexOutOfRangeException">
        /// 当tagId或则articleId没有对应的tag或文章时抛出该异常，例如tagId或articleId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public abstract Task AddTagForArticleAsync(int articleId, int tagId);

        /// <summary>
        /// delete a tag from an article
        /// </summary>
        /// <param name="articleId">文章Id</param>
        /// <param name="tagId">tag的Id</param>
        /// <exception cref="IndexOutOfRangeException">
        /// 当tagId或则articleId没有对应的tag或文章时抛出该异常，例如tagId或articleId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public abstract Task DeleteTagFromArticleAsync(int articleId, int tagId);

        // others

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns>
        ///  1: 保存成功
        ///  0: 失败
        /// </returns>
        public abstract Task<bool> SaveAsync();
    } 
}