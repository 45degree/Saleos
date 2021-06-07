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

using System.Diagnostics;
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