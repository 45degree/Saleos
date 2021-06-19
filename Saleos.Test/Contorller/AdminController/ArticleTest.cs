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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saleos.Controllers;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;
using Saleos.Models;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class ArticleTest : HomePageControllerTest
    {
        public ArticleTest() : base("Mock AdminController-Article-routin")
        {
        }

        [Theory]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public async Task Article_PageOutOfRange_RedirectToFirstPage(int pageId)
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new AdminController(articleServices);

            var result = await controller.Article(pageId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Article", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["page"]);
        }

        [Fact]
        public async Task Article_ValidPage_GetViewResult()
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new AdminController(articleServices);

            var result = await controller.Article(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AdminArticleViewModel>(viewResult.Model);
            Assert.Equal(3, model.articleInfos.Count);
        }
    }
}
