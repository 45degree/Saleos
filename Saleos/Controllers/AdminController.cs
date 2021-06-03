using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Saleos.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        // GET
        [Route("")]
        [Route("Article")]
        public IActionResult Article()
        {
            return View();
        }

        [Route("Tags")]
        public IActionResult Tags()
        {
            return View();
        }

        [Route("Category")]
        public IActionResult Category()
        {
            return View();
        }

        [Route("Editor")]
        public IActionResult Editor()
        {
            return View();
        }
    }
}