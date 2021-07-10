using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saleos.Entity.Services.CoreServices;
using Saleos.Model;

namespace Saleos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ArticleServices _articleSerivces;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(
            ArticleServices articleServices,
            ILogger<CategoryController> logger
        )
        {
            _articleSerivces = articleServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            _logger.LogInformation(
                $"{nameof(GetAllCategory)}: get a request;"
            );
            var categories = await _articleSerivces.CategoryRepository.GetCategoryAsync();
            return Ok(new CategoryModel
            {
                Data = categories,
            });
        }
    }
}
