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
    public class ArticleInfoDto
    {
        public int Id { get; set; }
        public bool IsReprint {get; set;}
        public string RerpintUrl {get; set;}
        public string Abstract { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; }
        public CategoryDto Category { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
