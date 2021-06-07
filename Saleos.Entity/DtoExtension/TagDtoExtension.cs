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

using Saleos.DTO;

namespace Saleos.Entity.DtoExtension
{
    public static class TagDtoExtension
    {
        public static TagDto GetTagDtoFromTag(this Tag tag)
        {
            var tagDto = new TagDto()
            {
                Id = tag.Id,
                Content = tag.Content,
            };
            return tagDto;
        }

        public static Tag GetTagFromTagAddDto(this TagAddDto addDto)
        {
            var tag = new Tag() { Content = addDto.Content };
            return tag;
        }
    }
}