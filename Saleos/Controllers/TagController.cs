using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Saleos.Entity.Services.CoreServices;
using Saleos.Model;

namespace Saleos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ArticleServices _articleServices;
        private readonly ILogger<TagController> _logger;

        public TagController(
            ArticleServices articleServices,
            ILogger<TagController> logger
        )
        {
            _articleServices = articleServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTag()
        {
            _logger.LogInformation(
                $"{nameof(GetAllTag)}: get a request;"
            );
            var tags = await _articleServices.TagRepository.GetTagAsync();
            return Ok(new TagModel
            {
                Data = tags,
            });
        }
    }
}
