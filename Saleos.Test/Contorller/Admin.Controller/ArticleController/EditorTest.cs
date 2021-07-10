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
    public class EditorTest : HomePageControllerTest
    {
        public EditorTest() : base("Mock Admin.ArticleController-Editor-routin")
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task Editor_ArticleIdOutOfRange_RedirectToNewPageEditor(int articleId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var result = await controller.Editor(articleId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditorPageViewModel>(viewResult.Model);
            Assert.Equal(0, model.Article.Id);
            Assert.Null(model.Article.Content);
            Assert.Null(model.Article.Tags);
            Assert.Null(model.Article.Category);
            Assert.Equal(2, model.Categories.Count);
            Assert.Equal(3, model.Tags.Count);
        }

        [Fact]
        public async Task Editor_ValidArticleId_GetViewResult()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var result = await controller.Editor(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<EditorPageViewModel>(viewResult.Model);
            Assert.Equal(1, model.Article.Id);
            Assert.Equal(2, model.Categories.Count);
            Assert.Equal(3, model.Tags.Count);
        }
    }
}
