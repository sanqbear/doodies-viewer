using System;

namespace DoodieViewer.Server.Service
{
    public class UrlBindService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public UrlBindService(ILogger<UrlBindService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;

            ServiceUrl = string.Empty;
        }


        public async Task<bool> BindServiceUrl()
        {
            if (IsRunning)
                return false;


            IsRunning = true;
            IsValid = false;
            ServiceUrl = string.Empty;

            bool isSuccess = false;


            try
            {
                string url = string.Empty;

                // try to get from cache
                url = await GetServiceUrlFromCache();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    ServiceUrl = url;
                    IsValid = true;
                    Timestamp = DateTimeOffset.Now;
                    isSuccess = true;
                    return isSuccess;
                }

                // else, try to find from the internet
                int index = 0;
                do
                {
                    try
                    {
                        string requestUrl = $"https://manatoki{index}.net";
                        url = await ValidateUrl(requestUrl);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }

                    if (!string.IsNullOrWhiteSpace(url))
                        break;
                    else
                        index++;

                    Thread.Sleep(100);
                }
                while (index < 1000 && string.IsNullOrWhiteSpace(url));

                if (!string.IsNullOrWhiteSpace(url))
                {
                    // if success, save to file
                    try
                    {
                        string fileName = configuration.GetValue<string>("ServiceUrl:CachedPath") ?? "service-url.txt";
                        string filePath = Path.IsPathFullyQualified(fileName) ? fileName : Path.Combine(AppContext.BaseDirectory, fileName);

                        File.WriteAllText(filePath, url, System.Text.Encoding.UTF8);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }

                    ServiceUrl = url;
                    IsValid = true;
                    Timestamp = DateTimeOffset.Now;
                    isSuccess = true;
                }
            }
            catch (Exception error)
            {
                logger.LogError(error, "Error occurred in FindApiUrl. {Message}", error.Message);
            }
            finally
            {
                IsRunning = false;
            }

            return isSuccess;
        }

        private async Task<string> GetServiceUrlFromCache()
        {
            string fileName = configuration.GetValue<string>("ServiceUrl:CachedPath") ?? "service-url.txt";
            string filePath = Path.IsPathFullyQualified(fileName) ? fileName : Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(filePath))
            {
                string url = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);

                if (!string.IsNullOrWhiteSpace(url))
                {
                    try
                    {
                        return await ValidateUrl(url);
                    }
                    catch (Exception) { }
                }
            }
            return string.Empty;
        }

        private async Task<string> ValidateUrl(string requestUrl)
        {
            string url = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.GetAsync(requestUrl);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        {
                            string serverHeader = response.Headers.Server?.ToString() ?? string.Empty;
                            if ("cloudflare".Equals(serverHeader, StringComparison.InvariantCultureIgnoreCase)
                                && !response.Headers.TryGetValues("Panel", out var _)
                                && !response.Headers.TryGetValues("Platform", out var _)
                                && !response.Headers.TryGetValues("Link", out var _))
                            {
                                url = response.RequestMessage?.RequestUri?.ToString() ?? string.Empty;

                                if (string.IsNullOrWhiteSpace(url))
                                {
                                    url = requestUrl;
                                }
                            }
                        }
                        break;
                    case System.Net.HttpStatusCode.Redirect:
                        {
                            string location = response.Headers.Location?.ToString() ?? string.Empty;

                            if (!string.IsNullOrWhiteSpace(location) && location.StartsWith("https") && location.Contains("manatoki"))
                            {
                                url = location;
                            }
                        }
                        break;
                }
            }
            return url;
        }



        public bool IsRunning { get; private set; }

        public string ServiceUrl { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public bool IsValid { get; private set; }

    }
}
