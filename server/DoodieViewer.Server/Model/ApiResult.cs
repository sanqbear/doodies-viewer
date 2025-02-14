using System.Text.Json.Serialization;

namespace DoodieViewer.Server.Model
{
    public class ApiResult
    {
        public bool Success { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Message { get; set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }
    }
}
