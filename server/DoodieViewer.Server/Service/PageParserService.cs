using DoodieViewer.Server.Model;
using DoodieViewer.Server.Model.Items;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace DoodieViewer.Server.Service
{
    public class PageParserService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private static readonly Regex LineBreakCharRegex = new Regex(@"\s", RegexOptions.Compiled);
        private static readonly Regex LikeCountRegex = new Regex(@"^[\d+\s]+", RegexOptions.Compiled);

        public PageParserService(ILogger<PageParserService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }


        public async Task<HomePageResult> GetHomePageData(string url)
        {
            List<ManhwaBase> recent = new List<ManhwaBase>();
            List<ManhwaBase> ranked = new List<ManhwaBase>();
            List<ManhwaBase> weekly = new List<ManhwaBase>();

            var page = await GetPage(url);
            if (page != null)
            {
                var firstGalleryDiv = page.DocumentNode.SelectSingleNode("//div[contains(@class, 'miso-post-gallery')]");
                if (firstGalleryDiv != null)
                {
                    var firstGalleryImages = firstGalleryDiv.SelectNodes("//img");
                    if (firstGalleryImages != null)
                    {
                        foreach (var e in firstGalleryDiv.SelectNodes(".//div[contains(@class, 'post-row')]"))
                        {
                            var anchor = e.SelectSingleNode(".//a");
                            int id = int.Parse(anchor.GetAttributeValue("href", "").Split(new[] { "comic/" }, StringSplitOptions.None)[1]);

                            var infos = e.SelectSingleNode(".//div[contains(@class, 'img-item')]");
                            var thumb = infos.SelectSingleNode(".//img").GetAttributeValue("src", "");
                            var name = infos.SelectSingleNode(".//b").InnerText.Trim();

                            recent.Add(new ManhwaBase(id, name, thumb));
                        }
                    }
                }

                // 인기 랭킹 수집
                var lastGalleryDiv = page.DocumentNode.SelectNodes("//div[contains(@class, 'miso-post-gallery')]").LastOrDefault();
                if (lastGalleryDiv != null)
                {
                    int i = 1;
                    foreach (var e in lastGalleryDiv.SelectNodes(".//div[contains(@class, 'post-row')]"))
                    {
                        var anchor = e.SelectSingleNode(".//a");
                        int id = int.Parse(anchor.GetAttributeValue("href", "").Split(new[] { "comic/" }, StringSplitOptions.None)[1]);

                        var infos = e.SelectSingleNode(".//div[contains(@class, 'img-item')]");
                        var thumb = infos.SelectSingleNode(".//img").GetAttributeValue("src", "");
                        var name = infos.SelectSingleNode(".//div[contains(@class, 'in-subject')]").InnerText.Trim();

                        ranked.Add(new ManhwaBase(id, name, thumb));
                    }
                }

                // 주간 랭킹 수집
                var lastPostListDiv = page.DocumentNode.SelectNodes("//div[contains(@class, 'miso-post-list')]").LastOrDefault();
                if (lastPostListDiv != null)
                {
                    int i = 1;
                    foreach (var e in lastPostListDiv.SelectNodes(".//li[contains(@class, 'post-row')]"))
                    {
                        var anchor = e.SelectSingleNode(".//a");
                        int id = int.Parse(anchor.GetAttributeValue("href", "").Split(new[] { "comic/" }, StringSplitOptions.None)[1]);
                        var name = anchor.InnerText.Trim();

                        if(!string.IsNullOrWhiteSpace(name))
                        {
                            name = LineBreakCharRegex.Replace(name, " ");
                            name = LikeCountRegex.Replace(name, "").Trim();
                        }

                        weekly.Add(new ManhwaBase(id, name));
                    }
                }
            }

            return new HomePageResult(recent, ranked, weekly);
        }

        private async Task<HtmlDocument?> GetPage(string url)
        {

            string host = new Uri(url).Host;
            string ipAddress = await ResolveDoh(host);

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                logger.LogError("Failed to resolve DOH");
                return default;
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
                    return document;
                }
                else
                {
                    return default;
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
                    if (result?.Answer != null && result.Answer.Length > 0)
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
