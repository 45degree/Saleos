using Xunit;
using Saleos.Controllers;
using Saleos.Entity.Services.CoreServices;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Saleos.DAO;
using Saleos.Models;

namespace Saleos.Test.Controller.HomeControllerTest
{
    public class IndexTest : HomePageControllerTest
    {
        public IndexTest() : base("Mock HomeController-Index-routin")
        {
        }

        [Theory]
        [InlineData(2)]
        [InlineData(int.MaxValue)]
        public async Task Index_PageNumberIsOutOfRange_RedirectToFirstIndexPage(int page)
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(articleServices, mockLogger.Object);

            var result = await controller.Index(page);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Index_SamplePageNumber_GetViewResult()
        {
            using var context = getContext();
            var articleServices = new ArticleServicesImpl(context);
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(articleServices, mockLogger.Object);

            var result = await controller.Index(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var articleInfos = Assert.IsAssignableFrom<HomeIndexViewModel>(viewResult.Model);
            Assert.Equal(3, articleInfos.articleInfos.Count);
        }
    }
}
