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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saleos.DTO;
using Saleos.Entity.Services.CoreServices;
using Saleos.Models;

namespace Saleos.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private ArticleServices _articleServices;
        public AdminController(ArticleServices articleServices)
        {
            _articleServices = articleServices;
        }

        [HttpGet]
        [Route("")]
        [Route("Article")]
        public async Task<IActionResult> Article([FromQuery]int page = 1)
        {
            var queryDto = new ArticlesQueryDto()
            {
                PageNumber = page,
                PageSize = 10,
            };
            var articleInfos = await _articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDto);
            return View(articleInfos);
        }

        [HttpDelete, Route("Article/{articleId:int}")]
        public async Task<IActionResult> DeleteArticle([FromRoute] int articleId)
        {
            if (!await _articleServices.ArticleRepository.ArticleIsExisted(articleId)) return NotFound();
            await _articleServices.ArticleRepository.DeleteArticleAsync(articleId);
            await _articleServices.SaveAsync();
            return NoContent();
        }

        // TODO need to improve
        [HttpPost, Route("Article")]
        public async Task<IActionResult> AddOrUpdateArticle([FromBody]EditorPagePostModel postModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Article)}");

            if (postModel.Id == 0)
            {
                // a new article
                var articleAddDto = new ArticleAddDto()
                {
                    Title = postModel.Title,
                    Content = postModel.Content,
                    CreateTime = DateTime.Now,
                    CategoryId = postModel.CategoryId,
                    Tags = postModel.NewTags,
                    IsReprint = postModel.IsReprint,
                    ReprintUri = postModel.ReprintUri,
                };
                await _articleServices.ArticleRepository.AddArticleAsync(articleAddDto);
                await _articleServices.SaveAsync();
            }
            else
            {
                // a existed article
                var articleUpdateDto = new ArticleUpdateDto()
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
                await _articleServices.ArticleRepository.UpdateArticleAsync(articleUpdateDto);
                await _articleServices.SaveAsync();
            }

            return RedirectToAction($"{nameof(Article)}");
        }

        [Route("Tags")]
        public async Task<IActionResult> Tags([FromQuery]int page = 1)
        {
            var queryDto = new TagQueryDto()
            {
                PageNumber = page,
                PageSize = 10
            };
            var tags = await _articleServices.TagRepository.GetTagsByQueryAsync(queryDto);
            var tagPageViewModel = new TagPageViewModel()
            {
                Tags = tags,
            };
            return View(tagPageViewModel);
        }

        [HttpPost]
        [Route("Tags")]
        public async Task<IActionResult> AddOrUpdateTags([FromBody] TagPagePostModel tagPagePostModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Tags)}");
            if (tagPagePostModel.Id == 0)
            {
                // a new tag
                var addTagDto = new TagAddDto()
                {
                    Content = tagPagePostModel.Content,
                };
                await _articleServices.TagRepository.AddTagAsync(addTagDto);
                await _articleServices.SaveAsync();
            }
            else
            {
                // a existed tag
                var updateTagDto = new TagUpdateDto()
                {
                    Id = tagPagePostModel.Id,
                    Content = tagPagePostModel.Content,
                };
                await _articleServices.TagRepository.UpdateTagAsync(updateTagDto);
                await _articleServices.SaveAsync();
            }
            return RedirectToAction($"{nameof(Tags)}");
        }

        [HttpDelete("Tags/{tagId:int}")]
        public async Task<IActionResult> DeleteTag([FromRoute] int tagId)
        {
            if (!await _articleServices.TagRepository.TagIsExistAsync(tagId)) return NotFound();
            await _articleServices.TagRepository.DeleteTagAsync(tagId);
            await _articleServices.SaveAsync();
            return NoContent();
        }

        [HttpGet]
        [Route("Category")]
        public async Task<IActionResult> Category([FromQuery]int page)
        {
            var queryDto = new CategoryQueryDto()
            {
                PageNumber = page,
                PageSize = 10
            };
            var categories = await _articleServices.CategoryRepository.GetCategoryByQueryAsync(queryDto);

            var categoryPageViewModel = new CategoryPageViewModel()
            {
                Categories = categories,
            };

            return View(categoryPageViewModel);
        }

        [HttpPost]
        [Route("Category")]
        public async Task<IActionResult> AddOrUpdateCategory([FromBody] CategoryPagePostModel categoryPagePostModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Category)}");
            if (categoryPagePostModel.Id == 0)
            {
                var categoryAddDto = new CategoryAddDto()
                {
                    Content = categoryPagePostModel.Content
                };
                await _articleServices.CategoryRepository.AddCategoryAsync(categoryAddDto);
                await _articleServices.SaveAsync();
            }
            else
            {
                var categoryUpdateDto = new CategoryUpdateDto()
                {
                    Id = categoryPagePostModel.Id,
                    Content = categoryPagePostModel.Content
                };
                await _articleServices.CategoryRepository.UpdateCategoryAsync(categoryUpdateDto);
                await _articleServices.SaveAsync();
            }

            return RedirectToAction($"{nameof(Category)}");
        }

        [HttpDelete("Category/{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int categoryId)
        {
            if (!await _articleServices.CategoryRepository.CategoryIsExistAsync(categoryId))
                return NotFound();

            await _articleServices.CategoryRepository.DeleteCategoryAsync(categoryId);
            await _articleServices.SaveAsync();
            return NoContent();
        }

        [Route("Editor")]
        public async Task<IActionResult> Editor([FromQuery]int articleId = 0)
        {
            var model = new EditorPageViewModel
            {
                Articles = new ArticleDto(),
                Tags = await _articleServices.TagRepository.GetTagAsync() ?? new List<TagDto>(),
                Categories = await _articleServices.CategoryRepository.GetCategoryAsync(),
            };
            if (await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                model.Articles = await _articleServices.ArticleRepository
                    .GetArticleAsync(articleId);
            }
            return View(model);
        }
    }
}
