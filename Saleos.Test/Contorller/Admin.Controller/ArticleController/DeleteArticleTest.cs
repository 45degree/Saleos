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
using Microsoft.EntityFrameworkCore;
using Saleos.Admin.Controllers;
using Saleos.Entity.Services.CoreServices;
using Xunit;

namespace Saleos.Test.Controller.AdminControllerTest
{
    public class DeleteArticleTest : HomePageControllerTest
    {
        public DeleteArticleTest() : base("Mock Admin.ArticleController-Delete-routin")
        {
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task Delete_articleIdIsOutOfRange_GetNotFound(int articleId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var result = await controller.DeleteArticle(articleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Delete_ValidArticleId_DeleteArticleInDatabase(int articleId)
        {
            using var context = GetContext();
            var articleServices = new ArticleServicesImpl(context);
            var controller = new ArticleController(articleServices);

            var result = await controller.DeleteArticle(articleId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(await context.Article.AnyAsync( x => x.Id == articleId));
        }
    }
}
