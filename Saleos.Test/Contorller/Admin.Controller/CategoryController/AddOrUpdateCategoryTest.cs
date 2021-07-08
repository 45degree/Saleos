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
using Saleos.Controllers;
using Saleos.Entity.Services.CoreServices;
using Saleos.Models;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class AddOrUpdateCategoryTest : HomePageControllerTest
    {
        public AddOrUpdateCategoryTest() : base("Mock AdminController-AddOrUpdateCategory-routin")
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task AddOrUpdateCategory_CategoryIdIsOutOfRange_GetNotFound(int categoryId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new CategoryController(articleServices);

            var model = new CategoryPagePostModel
            {
                Id = categoryId,
                Content = "new category",
            };

            var result = await controller.Index(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddOrUpdateCategory_UpdateValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new CategoryController(articleServices);

            var model = new CategoryPagePostModel
            {
                Id = 2,
                Content = "new Category",
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Content, context.Categories.SingleOrDefault(x => x.Id == 2).Content);
        }

        [Fact]
        public async Task AddOrUpdateCategory_AddValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new CategoryController(articleServices);

            var model = new CategoryPagePostModel
            {
                Id = 0,
                Content = "new Category",
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Content, context.Categories.SingleOrDefault(x => x.Id == 3).Content);
        }
    }
}
