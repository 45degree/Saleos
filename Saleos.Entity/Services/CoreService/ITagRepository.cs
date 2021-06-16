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
using Saleos.DAO;

namespace Saleos.Entity.Services.CoreServices
{
    public interface ITagRepository
    {
        /// <summary>
        /// 通过Id判断tag是否存在
        /// </summary>
        /// <param name="tagId">tag的Id</param>
        /// <returns>
        /// 如果存在该Tag，则返回true，否则返回false
        /// </returns>
        public Task<bool> TagIsExistAsync(int tagId);

        /// <summary>
        /// 获取Tag的数量
        /// </summary>
        /// <returns>
        /// 数据库中Tag的数量
        /// </returns>
        public Task<int> GetTagsCountAsync();

        /// <summary>
        /// 异步获取所有Tags
        /// </summary>
        /// <returns>
        /// Tag的集合
        /// </returns>
        public Task<List<TagDAO>> GetTagAsync();

        /// <summary>
        /// 通过id异步获取Tag
        /// </summary>
        /// <param name="tagId">Tag的id</param>
        /// <returns>id对应的Tag</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// 当tagId没有对应的tag时抛出该异常，例如tagId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public Task<TagDAO> GetTagAsync(int tagId);

        /// <summary>
        /// Get Tags by query
        /// </summary>
        /// <param name="tagQueryDAO"></param>
        /// <returns></returns>
        public Task<List<TagDAO>> GetTagsByQueryAsync(TagQueryDAO tagQueryDAO);

        /// <summary>
        /// 添加tag
        /// </summary>
        /// <param name="tag">需要被添加的tag</param>
        /// <exception cref="ArgumentNullException">
        /// tag为null时抛出该异常
        /// </exception>
        public Task AddTagAsync(TagAddDAO tag);

        /// <summary>
        /// 更新tag
        /// </summary>
        /// <param name="tag">更新后的tag</param>
        public Task UpdateTagAsync(TagUpdateDAO tag);

        /// <summary>
        /// 删除一个tag
        /// </summary>
        /// <param name="tagId">tag的Id</param>
        /// <exception cref="IndexOutOfRangeException">
        /// 当tagId没有对应的tag时抛出该异常，例如tagId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public Task DeleteTagAsync(int tagId);
    }
}
