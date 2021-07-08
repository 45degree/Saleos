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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Controllers;
using Saleos.Admin.Models;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class TagsTest : HomePageControllerTest
    {
        public TagsTest() : base("Mock AdminController-Tags-routin")
        {
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(int.MaxValue)]
        public async Task Tags_PageOutOfRange_RedirectToFirstPage(int pageId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new TagController(articleServices);

            var result = await controller.Index(pageId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["page"]);
        }

        [Fact]
        public async Task Tags_ValidPage_GetViewResult()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new TagController(articleServices);

            var result = await controller.Index(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TagPageViewModel>(viewResult.Model);
            Assert.Equal(3, model.Tags.Count);
        }
    }
}
