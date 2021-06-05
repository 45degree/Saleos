using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saleos.DTO;

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
        public Task<bool> TagIsExistedAsync(int tagId);

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
        public Task<List<TagDto>> GetTagAsync();

        /// <summary>
        /// 通过id异步获取Tag
        /// </summary>
        /// <param name="tagId">Tag的id</param>
        /// <returns>id对应的Tag</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// 当tagId没有对应的tag时抛出该异常，例如tagId小于等于0，或则大于数据库中最大Id
        /// </exception>
        public Task<Tag> GetTagAsync(int tagId);

        /// <summary>
        /// 添加tag
        /// </summary>
        /// <param name="tag">需要被添加的tag</param>
        /// <exception cref="ArgumentNullException">
        /// tag为null时抛出该异常
        /// </exception>
        public void AddTag(Tag tag);

        /// <summary>
        /// 更新tag
        /// </summary>
        /// <param name="tag">更新后的tag</param>
        public void UpdateTag(Tag tag);

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