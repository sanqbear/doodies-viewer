using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace DoodieViewer.Server.Middleware
{
    public class InternationalizationAttribute : ActionFilterAttribute
    {
        public InternationalizationAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? lang = context.HttpContext.Request.Query["lang"].ToString();

            if (string.IsNullOrWhiteSpace(lang))
                lang = context.RouteData.Values["lang"] as string;

            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = DefaultLanguage;
            }

            // ko는 북한, zh는 대만이므로, 적절한 territory 값을 부여한다
            switch (lang.ToLower())
            {
                case "ko":
                case "kr":
                    lang = "ko-KR";
                    break;
                case "en":
                case "us":
                    lang = "en-US";
                    break;
                case "zh":
                case "cn":
                    lang = "zh-CN";
                    break;
                case "ja":
                case "jp":
                    lang = "ja-JP";
                    break;
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);

            base.OnActionExecuting(context);
        }

        public string DefaultLanguage { get; set; } = "en";
    }
}
