using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Models;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;

namespace Saleos.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticleInfoController : Controller
    {
        private readonly ArticleServices _articleServices;
        public ArticleInfoController(ArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] int page = 1)
        {
            var queryDAO = new ArticlesQueryDAO()
            {
                PageNumber = page,
                PageSize = 10,
            };
            var articleInfos = await _articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDAO);

            if(articleInfos.Count == 0)
            {
                return RedirectToAction($"{nameof(Index)}", new {page = 1});
            }

            int articleCount = await _articleServices.ArticleRepository.GetArticleCountAsync();
            double maxPage = Convert.ToDouble(articleCount) / queryDAO.PageSize;

            var model = new AdminArticleInfoViewModel
            {
                ArticleInfos = articleInfos,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };
            return View(model);
        }

        [HttpPut]
        public async Task<IActionResult> Index(
            [FromBody] AdminArticleInfoUpdateModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction($"{nameof(Index)}", new { page = 1});
            }

            if( !await _articleServices.ArticleRepository.ArticleIsExisted(model.Id) )
            {
                return NotFound();
            }

            // new article info DAO
            var articleUpdateDAO = new ArticleInfoUpdateDAO
            {
                Id = model.Id,
                Title = model.Title,
                Abstract = model.Abstract,
                ImageUrl = model.ImageUrl,
                IsReprint = model.IsReprint,
                ReprintUrl = model.ReprintUrl,
            };

            await _articleServices.ArticleInfoRepository.UpdateArticleInfoAsync(articleUpdateDAO);
            await _articleServices.SaveAsync();
            return NoContent();
        }

        public async Task<IActionResult> ArticleInfoEditor([FromQuery] int articleId = 0)
        {
            if(!await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                return RedirectToAction($"{nameof(Index)}", new { page = 1 });
            }
            var model = new AdminArticleInfoEditorViewModel
            {
                ArticleInfo = await _articleServices.ArticleInfoRepository.GetArticleInfoAsync(articleId)
            };
            return View(model);
        }
    }
}
