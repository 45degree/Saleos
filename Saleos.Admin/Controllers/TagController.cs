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
    public class TagController : Controller
    {
        private readonly ArticleServices _articleServices;
        public TagController(ArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        public async Task<IActionResult> Index([FromQuery]int page = 1)
        {
            var queryDAO = new TagQueryDAO()
            {
                PageNumber = page,
                PageSize = 10
            };
            var tags = await _articleServices.TagRepository.GetTagsByQueryAsync(queryDAO);
            if(tags.Count == 0)
            {
                return RedirectToAction($"{nameof(Index)}", new {page = 1});
            }

            var tagCount = await _articleServices.TagRepository.GetTagsCountAsync();

            double maxPage = Convert.ToDouble(tagCount) / queryDAO.PageSize;
            var tagPageViewModel = new TagPageViewModel()
            {
                Tags = tags,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };
            return View(tagPageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            [FromBody] TagPagePostModel tagPagePostModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Index)}");
            if (tagPagePostModel.Id == 0)
            {
                // a new tag
                var addTagDAO = new TagAddDAO()
                {
                    Content = tagPagePostModel.Content,
                };
                await _articleServices.TagRepository.AddTagAsync(addTagDAO);
                await _articleServices.SaveAsync();
            }
            else
            {
                if(!await _articleServices.TagRepository.TagIsExistAsync(tagPagePostModel.Id))
                {
                    return NotFound();
                }

                // a existed tag
                var updateTagDAO = new TagUpdateDAO()
                {
                    Id = tagPagePostModel.Id,
                    Content = tagPagePostModel.Content,
                };
                await _articleServices.TagRepository.UpdateTagAsync(updateTagDAO);
                await _articleServices.SaveAsync();
            }
            return RedirectToAction($"{nameof(Index)}");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteTag([FromRoute] int tagId)
        {
            if (!await _articleServices.TagRepository.TagIsExistAsync(tagId))
            {
                return NotFound();
            }
            await _articleServices.TagRepository.DeleteTagAsync(tagId);
            await _articleServices.SaveAsync();
            return NoContent();
        }

    }
}
