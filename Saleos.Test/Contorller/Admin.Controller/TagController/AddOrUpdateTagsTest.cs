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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Controllers;
using Saleos.Admin.Models;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class AddOrUpdateTagsTest : HomePageControllerTest
    {
        public AddOrUpdateTagsTest() : base("Mock Admin.TagController-AddOrUpdateTags-routin")
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task AddOrUpdateTags_TagIdIsOutOfRange_GetNotFound(int tagId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new TagController(articleServices);

            var model = new TagPagePostModel
            {
                Id = tagId,
                Content = "new Content"
            };

            var result = await controller.Index(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddOrUpdateTags_UpdateValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new TagController(articleServices);

            var model = new TagPagePostModel
            {
                Id = 3,
                Content = "new Content",
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Content, context.Tags.SingleOrDefault(x => x.Id == 3).Content);
        }

        [Fact]
        public async Task AddOrUpdateTags_AddValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new TagController(articleServices);

            var model = new TagPagePostModel
            {
                Id = 0,
                Content = "new Title",
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Content, context.Tags.SingleOrDefault(x => x.Id == 4).Content);
        }
    }
}
