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
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Minio;

namespace Saleos.Entity.Services.ImageStorage
{
    public class MinioImageStorage : IImageStorage
    {
        private readonly MinioClient _minioClient;
        private readonly string _bucketName;

        public MinioImageStorage(IConfiguration configuration)
        {
            _minioClient = new MinioClient(configuration["Minio:endpoint"],
                configuration["Minio:accessKey"],
                configuration["Minio:security"]);

            _bucketName = configuration["Minio:bucketName"] ?? "article";

            // check if the bucket exists in Minio, if not exists, create it
            var BucketExistsTask = _minioClient.BucketExistsAsync(_bucketName).GetAwaiter().GetResult();
            if(BucketExistsTask) {
                _minioClient.MakeBucketAsync(_bucketName).GetAwaiter();
            }
        }

        public async Task<byte[]> GetImageAsync(string imageName)
        {
            var imageMemoryStream = new MemoryStream();
            await _minioClient.GetObjectAsync(_bucketName, imageName, (stream) => {
                stream.CopyToAsync(imageMemoryStream);
            });
            return imageMemoryStream.ToArray();
        }

        public async Task<bool> IsImageExisted(string imageName)
        {
            var items = _minioClient.ListObjectsAsync(_bucketName);
            return await items.Any(x => x.Key == imageName);
        }

        public async Task UploadImageAsync(string imageName, Stream imageStream)
        {
            await _minioClient.PutObjectAsync(_bucketName, imageName,
                imageStream, imageStream.Length, "image/*");
        }
    }
}
