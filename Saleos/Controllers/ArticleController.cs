using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saleos.Entity.Services.CoreServices;
using Saleos.Model;

namespace Saleos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleServices _articleServices;
        private readonly ILogger<ArticleController> _logger;

        public ArticleController(
            ArticleServices articleServices,
            ILogger<ArticleController> logger)
        {
            _articleServices = articleServices;
            _logger = logger;
        }

        [HttpGet]
        [Route("{articleId:int}")]
        public async Task<IActionResult> GetArticleById(int articleId)
        {
            _logger.LogInformation(
                $"{nameof(GetArticleById)}: get a request;\r\n" +
                $"{nameof(articleId)}: {articleId}"
            );
            if(! await _articleServices.ArticleRepository.ArticleIsExisted(articleId))
            {
                _logger.LogWarning($"{nameof(GetArticleById)}: {articleId} is not existed;");
                return NotFound();
            }

            var article = await _articleServices.ArticleRepository.GetArticleAsync(articleId);

            return Ok(new ArticleModel
            {
                Data = article
            });
        }
    }
}
