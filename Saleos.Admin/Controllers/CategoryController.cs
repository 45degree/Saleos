
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
    public class CategoryController : Controller
    {
        private readonly ArticleServices _articleServices;
        public CategoryController(ArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery]int page = 1)
        {
            var queryDAO = new CategoryQueryDAO()
            {
                PageNumber = page,
                PageSize = 10
            };
            var categories = await _articleServices.CategoryRepository
                .GetCategoryByQueryAsync(queryDAO);

            if(categories.Count == 0)
            {
                return RedirectToAction($"{nameof(Index)}", new {page = 1});
            }

            var categoryCount = await _articleServices.CategoryRepository.GetCategoryCountAsync();
            double maxPage = Convert.ToDouble(categoryCount) / queryDAO.PageSize;

            var categoryPageViewModel = new CategoryPageViewModel()
            {
                Categories = categories,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };

            return View(categoryPageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            [FromBody] CategoryPagePostModel categoryPagePostModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction($"{nameof(Index)}");
            }
            if (categoryPagePostModel.Id == 0)
            {
                var categoryAddDAO = new CategoryAddDAO()
                {
                    Content = categoryPagePostModel.Content
                };
                await _articleServices.CategoryRepository.AddCategoryAsync(categoryAddDAO);
                await _articleServices.SaveAsync();
            }
            else
            {
                if(!await _articleServices.CategoryRepository
                    .CategoryIsExistAsync(categoryPagePostModel.Id))
                {
                    return NotFound();
                }
                var categoryUpdateDAO = new CategoryUpdateDAO()
                {
                    Id = categoryPagePostModel.Id,
                    Content = categoryPagePostModel.Content
                };
                await _articleServices.CategoryRepository.UpdateCategoryAsync(categoryUpdateDAO);
                await _articleServices.SaveAsync();
            }

            return RedirectToAction($"{nameof(Index)}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCategory([FromRoute]int categoryId)
        {
            if (!await _articleServices.CategoryRepository.CategoryIsExistAsync(categoryId))
                return NotFound();

            await _articleServices.CategoryRepository.DeleteCategoryAsync(categoryId);
            await _articleServices.SaveAsync();
            return NoContent();
        }
    }
}
