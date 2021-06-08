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

namespace Saleos.DTO
{
    public class ArticleDto : ArticleInfoDto, ICloneable
    {
        public string Content { get; set; }

        public object Clone()
        {
            var articleDto = new ArticleDto()
            {
                Id = Id,
                Title = Title,
                Abstract = Abstract,
                ImgUrl = ImgUrl,
                Category = Category,
                Content = Content,
                CreateTime = CreateTime,
                LastModifiedTime = LastModifiedTime,
                Tags = new List<TagDto>(),
            };

            foreach (var tagDto in Tags)
            {
                articleDto.Tags.Add(tagDto.Clone() as TagDto);
            }

            return articleDto;
        }
    }
}
