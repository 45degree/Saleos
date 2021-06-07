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

using System.Threading.Tasks;

namespace Saleos.Entity.Services.CoreServices
{
    public abstract class ArticleServices
    {
        /*  ArticleInfo */
        public IArticleInfoRepository ArticleInfoRepository { get; protected init; }

        /* tag */
        public ITagRepository TagRepository { get; protected init; }

        /* Article */
        public IArticleRepository ArticleRepository { get; protected init; }
        
        /* Category */
        public ICategoryRepository CategoryRepository { get; protected init; }

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