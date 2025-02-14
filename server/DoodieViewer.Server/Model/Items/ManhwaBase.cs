using System.Text.Json.Serialization;

namespace DoodieViewer.Server.Model.Items
{
    public class ManhwaBase
    {
        public ManhwaBase()
        {
            Id = -1;
            Name = string.Empty;
        }

        public ManhwaBase(int id, string name, string? thumbnail = default)
        {
            Id = id;
            Name = name;
            Thumbnail = thumbnail;
        }

        public int Id { get; set; }


        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Thumbnail { get; set; }
    }
}
