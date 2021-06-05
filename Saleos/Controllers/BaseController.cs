using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saleos.Entity.Services.CoreServices;

namespace Saleos.Controllers
{
    /// <summary>
    /// this controller is used to initial the common data in _Layout.html.
    /// all controllers that use _Layout.html should inherit this class
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected ArticleServices ArticleServices;
        
        public BaseController(ArticleServices articleServices)
        {
            ArticleServices = articleServices;
        }
        
        public override async void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO initial the UserInfo's info
            ViewData[""] = "";
            
            // TODO initial the TagsList's info
            ViewData["Tags"] = await ArticleServices.TagRepository.GetTagAsync();
            
            base.OnActionExecuted(context);
        }
    }
}