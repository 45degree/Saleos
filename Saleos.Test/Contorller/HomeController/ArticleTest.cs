using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Saleos.Controllers;
using Saleos.Entity.Services.CoreServices;
using Saleos.Models;
using Xunit;

namespace Saleos.Test.Controller.HomeControllerTest
{
    public class ArticleTest : HomePageControllerTest
    {
        public ArticleTest() : base("Mock HomeController-Article-routin")
        {
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(4)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public async Task Article_ArticleIdIsOutOfRange_RedirectToFirstArticlePage(int articleId)
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(articleServices, mockLogger.Object);

            var result = await controller.Article(-1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Article", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues["articleId"]);
        }

        [Fact]
        public async Task Article_ValidArticleId_GetViewResult()
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(articleServices, mockLogger.Object);

            var result = await controller.Article(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ArticleViewModel>(viewResult.Model);
            Assert.Equal(1, model.article.Id);
        }
    }
}
