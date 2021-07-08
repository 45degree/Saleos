using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saleos.Admin.Models;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;

namespace Saleos.Admin.Controllers {

    [Authorize(Roles = "Admin")]
    public class ArticleController : Controller {
        private readonly ArticleServices _articleServices;
        public ArticleController(ArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]int page = 1)
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

            var model = new AdminArticleViewModel
            {
                ArticleInfos = articleInfos,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };
            return View(model);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArticle([FromRoute] int articleId)
        {
            if (!await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                return NotFound();
            }
            await _articleServices.ArticleRepository.DeleteArticleAsync(articleId);
            await _articleServices.SaveAsync();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody]EditorPagePostModel postModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Index)}");

            foreach(var tagId in postModel.NewTags)
            {
                if(!await _articleServices.TagRepository.TagIsExistAsync(tagId))
                {
                    return NotFound();
                }
            }

            if (postModel.Id == 0)
            {
                // a new article
                var articleAddDAO = new ArticleAddDAO()
                {
                    Title = postModel.Title,
                    Content = postModel.Content,
                    CreateTime = DateTime.Now,
                    CategoryId = postModel.CategoryId,
                    Tags = postModel.NewTags,
                    IsReprint = postModel.IsReprint,
                    ReprintUri = postModel.ReprintUri,
                };
                await _articleServices.ArticleRepository.AddArticleAsync(articleAddDAO);
                await _articleServices.SaveAsync();
            }
            else
            {
                if(!await _articleServices.ArticleRepository.ArticleIsExisted(postModel.Id))
                {
                    return NotFound();
                }

                // a existed article
                var articleUpdateDAO = new ArticleUpdateDAO()
                {
                    Id = postModel.Id,
                    Title = postModel.Title,
                    Content = postModel.Content,
                    LastModifiedTime = DateTime.Now,
                    Tags = postModel.NewTags,
                    CategoryId = postModel.CategoryId,
                    IsReprint = postModel.IsReprint,
                    ReprintUri = postModel.ReprintUri,
                };
                await _articleServices.ArticleRepository.UpdateArticleAsync(articleUpdateDAO);
                await _articleServices.SaveAsync();
            }

            return RedirectToAction($"{nameof(Index)}");
        }

        public async Task<IActionResult> Editor([FromQuery]int articleId = 0)
        {
            var model = new EditorPageViewModel
            {
                Article = new ArticleDAO(),
                Tags = await _articleServices.TagRepository.GetTagAsync() ?? new List<TagDAO>(),
                Categories = await _articleServices.CategoryRepository.GetCategoryAsync(),
            };
            if (await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                model.Article = await _articleServices.ArticleRepository
                    .GetArticleAsync(articleId);
            }
            return View(model);
        }
    }
}
