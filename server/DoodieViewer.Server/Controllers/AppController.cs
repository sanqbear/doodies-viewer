using Asp.Versioning;
using DoodieViewer.Server.Middleware;
using DoodieViewer.Server.Model;
using DoodieViewer.Server.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DoodieViewer.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/app")]
    [Internationalization]
    public class AppController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly UrlBindService urlBindService;

        public AppController(ILogger<AppController> logger, IConfiguration configuration, UrlBindService urlBindService)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.urlBindService = urlBindService;
        }


        [HttpGet]
        [Route("ping")]
        public IActionResult CheckServer(string? lang)
        {
            return Ok("pong");
        }

        [HttpPost]
        [Route("service-url")]
        public IActionResult RequestBindServiceUrl(string? lang)
        {
            ApiResult<RequestBindServiceUrlResponse> result = new ApiResult<RequestBindServiceUrlResponse>();

            if (urlBindService.IsRunning)
            {
                result.Success = true;
                result.Data = new RequestBindServiceUrlResponse(false, Properties.Resources.ALREADY_RUNNING);
            }
            else
            {
                urlBindService.BindServiceUrl().ConfigureAwait(false);
                result.Success = true;
                result.Data = new RequestBindServiceUrlResponse(true, Properties.Resources.WORKING_ON_IT);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("service-url")]
        public IActionResult GetServiceUrl(string? lang)
        {
            ApiResult<GetServiceUrlResponse> result = new ApiResult<GetServiceUrlResponse>();
            if (urlBindService.IsValid && TimeSpan.FromMilliseconds(Expires) > DateTimeOffset.Now - urlBindService.Timestamp)
            {
                result.Success = true;
                result.Data = new GetServiceUrlResponse(Success: true, StateCode: 0, Url: urlBindService.ServiceUrl);
            }
            else if (urlBindService.IsRunning)
            {
                result.Success = true;
                result.Data = new GetServiceUrlResponse(Success: false, StateCode: 1, Message: Properties.Resources.ALREADY_RUNNING);
            }
            else
            {
                result.Success = true;
                result.Data = new GetServiceUrlResponse(Success: false, StateCode: 2, Message: Properties.Resources.INVALID_URL);
            }
            return Ok(result);
        }

        public record RequestBindServiceUrlResponse(bool Success, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] string? Message = default);

        public record GetServiceUrlResponse(bool Success, int StateCode, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] string? Message = default, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] string? Url = default);


        public int Expires => configuration.GetValue<int>("ServiceUrl:Expires", 86400000);
    }
}
