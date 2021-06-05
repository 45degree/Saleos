using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saleos.DTO;
using Saleos.Entity.Services.CoreServices;
using Saleos.Models;

namespace Saleos.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ArticleServices articleServices, ILogger<HomeController> logger)
            : base(articleServices)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page)
        {
            var articlesQueryDto = new ArticlesQueryDto()
            {
                PageNumber = page
            };
            var articleInfos =  await ArticleServices.ArticleInfoRepository.GetArticleInfoByQueryAsync(articlesQueryDto);
            return View(articleInfos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}