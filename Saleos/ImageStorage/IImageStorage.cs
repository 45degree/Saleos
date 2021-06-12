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

using System.IO;
using System.Threading.Tasks;

namespace Saleos.ImageStorage
{
    public interface IImageStorage
    {
        public Task<byte[]> getImageAsync(string imageName);

        /// <summary>
        ///
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        public Task UploadImageAsync(string imageName, Stream imageStream);

        /// <summary>
        /// Returns true if the specified imageName is existed,
        /// otherwise return false
        /// </summary>
        /// <param name="imageName">image's Name</param>
        /// <returns>
        /// Task That return true if exists
        /// </returns>
        public Task<bool> IsImageExisted(string imageName);
    }
}
