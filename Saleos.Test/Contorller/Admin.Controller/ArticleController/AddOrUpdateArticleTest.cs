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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Controllers;
using Saleos.Entity.Services.CoreServices;
using Saleos.Admin.Models;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class AddOrUpdateArticleTest : HomePageControllerTest
    {
        public AddOrUpdateArticleTest()
            : base("Mock Admin.ArticleController-AddOrUpdateArticle-routin")
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task AddOrUpdateArticle_UpdateIdIsOutOfRange_GetNotFound(int articleId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var model = new EditorPagePostModel
            {
                Id = articleId,
                Title = "new Title",
                NewTags = new List<int>() {1, 2, 3},
            };

            var result = await controller.Index(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1, 2, 4)]
        [InlineData(-1, 2, 4)]
        [InlineData(0, 2, int.MaxValue)]
        [InlineData(1, 2, int.MinValue)]
        public async Task AddOrUpdateArticle_TagIdIsOutOfRange_GetNotFound(params int[] tagIds)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var model = new EditorPagePostModel
            {
                Id = 3,
                Title = "new Title",
                NewTags = new List<int>(tagIds),
            };

            var result = await controller.Index(model);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddOrUpdateArticle_UpdateValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var model = new EditorPagePostModel
            {
                Id = 3,
                Title = "new Title",
                NewTags = new List<int>(),
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Title, context.Article.SingleOrDefault(x => x.Id == 3).Title);
        }

        [Fact]
        public async Task AddOrUpdateArticle_AddValidModel_RedirectToArticleAction()
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var model = new EditorPagePostModel
            {
                Id = 0,
                Title = "new Title",
                NewTags = new List<int>(),
            };

            var result = await controller.Index(model);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectResult.ActionName);

            Assert.Equal(model.Title, context.Article.SingleOrDefault(x => x.Id == 4).Title);
        }
    }
}
