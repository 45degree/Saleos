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
using Newtonsoft.Json.Linq;
using Saleos.ImageStorage;

namespace Saleos.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IImageStorage _imageStorage;

        public ImageController(ILogger<ImageController> logger,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            IImageStorage imageStorage)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _imageStorage = imageStorage;
        }

        [HttpPost("upload/{articleId:int}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpLoadArticleImage(
            [FromForm]IFormFile files, int articleId)
        {
            _logger.LogInformation($"{nameof(UpLoadArticleImage)}: get a post request;");

            if (files == null)
            {
                _logger.LogError($"{nameof(UpLoadArticleImage)}: files is Null;");
                return BadRequest();
            }

            if (files.Length <= 0)
            {
                _logger.LogError($"{nameof(UpLoadArticleImage)}: the file length is less than 0;");
                return BadRequest();
            }

            _logger.LogInformation($"{nameof(UpLoadArticleImage)}: " +
                $"the file type is {files.ContentType}");

            string newFileName, webFile;
            using(var md5MemStream = new MemoryStream())
            {
                await files.CopyToAsync(md5MemStream);
                var md5Bytes = md5MemStream.ToArray();
                // calculate md5
                var md5 = MD5.Create();
                var hashcode = string.Join(string.Empty,
                    md5.ComputeHash(md5Bytes).Select(x => x.ToString("x2")));


                // generate new file name
                // the file name formate is [articleId]/[fileMd5].{png, jpg, ...}
                var file = files.FileName.Split('.');
                file[0] = hashcode;
                newFileName = string.Join(string.Empty,file[0], '.', file[1]);
                newFileName = Path.Join($"{articleId}/", newFileName);
                webFile = Path.Join("/api/image", newFileName);
            };

            using (var minioStream = new MemoryStream())
            {
                await files.CopyToAsync(minioStream);
                minioStream.Position = 0;
                // if the image don't exist, upload it
                if(!await _imageStorage.IsImageExisted(newFileName))
                {
                    await _imageStorage.UploadImageAsync(newFileName, minioStream);
                }
            };

            var json = GenerateImageReturnDto(files.Name, webFile);
            return Ok(json);
        }

        [HttpGet("{articleId:int}/{imgName}")]
        public async Task<IActionResult> GetImage([FromRoute]int articleId,
            [FromRoute]string imgName)
        {
            var imageByte = await _imageStorage.getImageAsync(Path.Join($"{articleId}", imgName));
            return base.File(imageByte, "image/*");
        }

        /// <summary>
        /// generate return date, which is needed by Vditor
        /// </summary>
        /// <see cref="https://b3log.org/vditor/demo/advanced-upload.html"/>
        /// <see cref="https://ld246.com/article/1549638745630#options-upload"/>
        private JObject GenerateImageReturnDto(string originalFile, string webFile)
        {
            var json = new JObject {{"msg", ""}, {"code", 0}};
            var data = new JObject();
            var succMap = new JObject {{originalFile, webFile}};

            data.Add("errFiles", null);
            data.Add("succMap", succMap);
            json.Add("data", data);
            return json;
        }
    }
}
