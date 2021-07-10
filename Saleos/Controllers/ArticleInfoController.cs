using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;
using Saleos.Model;

namespace Saleos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleInfoController : ControllerBase
    {
        private readonly ArticleServices _articleServices;
        private readonly ILogger<ArticleInfoController> _logger;

        public ArticleInfoController(
            ArticleServices articleServices,
            ILogger<ArticleInfoController> logger
        )
        {
            _articleServices = articleServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticleInfoByQuery(
            [FromQuery] ArticleInfoQueryModel articleInfoQueryModel
        )
        {
            _logger.LogInformation(
                $"{nameof(GetArticleInfoByQuery)}: get a request;\r\n" +
                $"{nameof(articleInfoQueryModel.Page)}: {articleInfoQueryModel.Page}\r\n" +
                $"{nameof(articleInfoQueryModel.PageSize)}: {articleInfoQueryModel.PageSize}"
            );

            var queryDAO = new ArticlesQueryDAO
            {
                PageNumber = articleInfoQueryModel.Page,
                PageSize = articleInfoQueryModel.PageSize,
            };
            var articleInfos = await _articleServices.ArticleInfoRepository
                .GetArticleInfoByQueryAsync(queryDAO);
            return Ok(new ArticleInfoModel
            {
                Data = articleInfos,
            });
        }
    }
}
