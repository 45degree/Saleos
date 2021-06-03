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
        public static string AdminArticleNavClass(ViewContext context) => IsActive(context, _adminArticle);

        private static string _adminTags = "AdminTags";
        public static string AdminTagsNavClass(ViewContext context) => IsActive(context, _adminTags);

        private static string _adminCategory = "AdminCategory";
        public static string AdminCategoryNavClass(ViewContext context) => IsActive(context, _adminCategory);

        private static string IsActive(ViewContext context, string page)
        {
            var activePage = context.ViewData["ActiveData"] as string ?? 
                System.IO.Path.GetFileNameWithoutExtension(context.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}