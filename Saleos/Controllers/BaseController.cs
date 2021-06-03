using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Saleos.Controllers
{
    /// <summary>
    /// this controller is used to initial the common data in _Layout.html.
    /// all controllers that use _Layout.html should inherit this class
    /// </summary>
    public abstract class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO initial the UserInfo's info
            ViewData[""] = "";
            
            // TODO initial the TagsList's info
            ViewData["Tags"] = "";
            
            base.OnActionExecuted(context);
        }
    }
}