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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Saleos.Views.Shared
{

    /// <summary>
    /// this class is used to toggle the active item in the sideBar.
    /// All Pages using _AdminLayout.cshtml should define ViewData["ActiveData"] and its value must be equal to
    /// one of the item's name in this class. For example: ViewData["ActiveData"] = "AdminArticle" or
    /// ViewBag.ActiveData = "AdminArticle"
    /// </summary>
    /// <see href="_SideBarPartial.cshtml"/>
    public static class SideBarPage
    {
        private static string _adminArticle = "AdminArticle";
        public static string AdminArticleNavClass(ViewContext context) =>
            IsActive(context, _adminArticle);

        private static string _adminTags = "AdminTags";
        public static string AdminTagsNavClass(ViewContext context) =>
            IsActive(context, _adminTags);

        private static string _adminCategory = "AdminCategory";
        public static string AdminCategoryNavClass(ViewContext context) =>
            IsActive(context, _adminCategory);

        private static string _adminArticleInfo = "AdminArticleInfo";
        public static string AdminArticleInfoNavClass(ViewContext context) =>
            IsActive(context, _adminArticleInfo);

        private static string IsActive(ViewContext context, string page)
        {
            var activePage = context.ViewData["ActiveData"] as string ??
                System.IO.Path.GetFileNameWithoutExtension(context.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ?
                "active" : null;
        }
    }
}
