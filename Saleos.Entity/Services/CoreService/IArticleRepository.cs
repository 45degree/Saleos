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
using System.Threading.Tasks;
using Saleos.DTO;

namespace Saleos.Entity.Services.CoreServices
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
        public Task<ArticleDto> GetArticleAsync(int articleId);

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
        public Task AddArticleAsync(ArticleAddDto article);

        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="article">修改后的文章</param>
        public Task UpdateArticleAsync(ArticleUpdateDto article);

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