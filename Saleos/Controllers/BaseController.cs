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

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;

namespace Saleos.Controllers
{
    /// <summary>
    /// this controller is used to initial the common data in _Layout.html.
    /// all controllers that use _Layout.html should inherit this class
    /// </summary>
    // public abstract class BaseController : Controller
    // {
    //     protected ArticleServices ArticleServices;

    //     protected BaseController(ArticleServices articleServices)
    //     {
    //         ArticleServices = articleServices;
    //     }

    //     public override void OnActionExecuted(ActionExecutedContext context)
    //     {
    //         // TODO initial the UserInfo's info
    //         ViewData[""] = "";

    //         // TODO initial the TagsList's info
    //         ViewData["Tags"] = ArticleServices.TagRepository.GetTagAsync().GetAwaiter().GetResult();

    //         base.OnActionExecuted(context);
    //     }
    // }
}
