using DoodieViewer.Server.Middleware;
using DoodieViewer.Server.Model;
using DoodieViewer.Server.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DoodieViewer.Server.Controllers
{
    /// <summary>
    /// 페이지에 필요한 데이터를 전달받음
    /// </summary>
    [ApiController]
    [Internationalization]
    [Route("api/v{version:apiVersion}/page")]
    public class PageController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly PageParserService pageParserService;
        private readonly UrlBindService urlBindService;

        public PageController(ILogger<PageController> logger, IConfiguration configuration, PageParserService pageParserService, UrlBindService urlBindService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.pageParserService = pageParserService;
            this.urlBindService = urlBindService;
        }

        [HttpGet]
        [Route("home")]
        public async Task<IActionResult> GetHomePage(string? lang)
        {
            ApiResult<GetHomePageResponse> result = new ApiResult<GetHomePageResponse>();

            if (string.IsNullOrWhiteSpace(urlBindService.ServiceUrl))
            {
                result.Success = true;
                result.Data = new GetHomePageResponse(Success: false, Message: Properties.Resources.INVALID_URL);
            }
            else
            {
                var data = await pageParserService.GetHomePageData(urlBindService.ServiceUrl);
                result.Success = true;
                result.Data = new GetHomePageResponse(Success: true, Items: data);
            }

            return Ok(result);
        }

        public record GetHomePageResponse(bool Success, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] HomePageResult? Items = default, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] string? Message = default);
    }
}
