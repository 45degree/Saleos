using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Saleos.Entity.Services.ImageStorage;

namespace Saleos.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly IImageStorage _imageStorage;

        public ImageController(ILogger<ImageController> logger,
            IImageStorage imageStorage)
        {
            _logger = logger;
            _imageStorage = imageStorage;
        }


        [HttpGet("{articleId:int}/{imgName}")]
        public async Task<IActionResult> GetImage([FromRoute]int articleId,
            [FromRoute]string imgName)
        {
            _logger.LogInformation(
                $"{nameof(GetImage)}: get a request;\r\n" +
                $"{nameof(articleId)} is {articleId}"
            );
            var imageByte = await _imageStorage.GetImageAsync($"{articleId}/{imgName}");
            return base.File(imageByte, "image/*");
        }
    }
}
