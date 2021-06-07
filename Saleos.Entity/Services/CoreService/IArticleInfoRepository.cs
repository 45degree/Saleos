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