using HtmlAgilityPack;
using System.Net;

namespace DoodieViewer.Server.Service
{
    public class PageParserService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public PageParserService(ILogger<PageParserService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }


        public async Task GetHomePageData(string url)
        {
            string host = new Uri(url).Host;
            string ipAddress = await ResolveDoh(host);

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                logger.LogError("Failed to resolve DOH");
                return;
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Host", host);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                var response = await client.GetAsync(url.Replace(host, ipAddress));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var document = new HtmlDocument();
                    document.LoadHtml(content);
                }
            }
        }

        private async Task<string> ResolveDoh(string hostName)
        {
            string ip = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/dns-json");

                    var response = await client.GetAsync($"https://cloudflare-dns.com/dns-query?name={hostName}&type=A");

                    var result = await response.Content.ReadFromJsonAsync<DohResponse>();
                    if(result?.Answer != null && result.Answer.Length > 0)
                    {
                        return result.Answer[0].Data ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to resolve DOH");
            }

            return ip;
        }

        public record DohResponse(int? Status, bool? TC, bool? RD, bool? RA, bool? AD, bool? CD, DohQuestion[] Question, DohAnswer[] Answer, string? Comment);

        public record DohQuestion(string? Name, int? Type);

        public record DohAnswer(string? Name, int? Type, int? TTL, string? Data);

    }
}
