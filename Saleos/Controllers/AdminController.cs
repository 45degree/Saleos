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
using Saleos.DAO;
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
            var queryDAO = new ArticlesQueryDAO()
            {
                PageNumber = page,
                PageSize = 10,
            };
            var articleInfos = await _articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDAO);

            if(articleInfos.Count == 0)
            {
                return RedirectToAction($"{nameof(Article)}", new {page = 1});
            }

            int articleCount = await _articleServices.ArticleRepository.GetArticleCountAsync();
            double maxPage = Convert.ToDouble(articleCount) / queryDAO.PageSize;

            var model = new AdminArticleViewModel
            {
                articleInfos = articleInfos,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };
            return View(model);
        }

        [HttpDelete, Route("Article/{articleId:int}")]
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

        [HttpPost, Route("Article")]
        public async Task<IActionResult> AddOrUpdateArticle([FromBody]EditorPagePostModel postModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Article)}");

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

            return RedirectToAction($"{nameof(Article)}");
        }

        [HttpGet]
        [Route("ArticleInfo")]
        public async Task<IActionResult> ArticleInfo([FromQuery] int page = 1)
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
                return RedirectToAction($"{nameof(ArticleInfo)}", new {page = 1});
            }

            int articleCount = await _articleServices.ArticleRepository.GetArticleCountAsync();
            double maxPage = Convert.ToDouble(articleCount) / queryDAO.PageSize;

            var model = new AdminArticleInfoViewModel
            {
                articleInfos = articleInfos,
                CurrentPage = page,
                MaxPage = (int)Math.Ceiling(maxPage),
            };
            return View(model);
        }

        [HttpPut]
        [Route("ArticleInfo")]
        public async Task<IActionResult> UpdateArticleInfo(
            [FromBody] AdminArticleInfoUpdateModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction($"{nameof(ArticleInfo)}", new { page = 1});
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

        [Route("Tags")]
        public async Task<IActionResult> Tags([FromQuery]int page = 1)
        {
            var queryDAO = new TagQueryDAO()
            {
                PageNumber = page,
                PageSize = 10
            };
            var tags = await _articleServices.TagRepository.GetTagsByQueryAsync(queryDAO);
            if(tags.Count == 0)
            {
                return RedirectToAction($"{nameof(Tags)}", new {page = 1});
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
        [Route("Tags")]
        public async Task<IActionResult> AddOrUpdateTags(
            [FromBody] TagPagePostModel tagPagePostModel)
        {
            if (!ModelState.IsValid) return RedirectToAction($"{nameof(Tags)}");
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
            return RedirectToAction($"{nameof(Tags)}");
        }

        [HttpDelete("Tags/{tagId:int}")]
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

        [HttpGet]
        [Route("Category")]
        public async Task<IActionResult> Category([FromQuery]int page = 1)
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
                return RedirectToAction($"{nameof(Category)}", new {page = 1});
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
        [Route("Category")]
        public async Task<IActionResult> AddOrUpdateCategory(
            [FromBody] CategoryPagePostModel categoryPagePostModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction($"{nameof(Category)}");
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

        [Route("ArticleInfoEditor")]
        public async Task<IActionResult> ArticleInfoEditor([FromQuery] int articleId = 0)
        {
            if(!await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                return RedirectToAction($"{nameof(ArticleInfo)}", new { page = 1 });
            }
            var model = new AdminArticleInfoEditorViewModel
            {
                articleInfo = await _articleServices.ArticleInfoRepository.GetArticleInfoAsync(articleId)
            };
            return View(model);
        }
    }
}
