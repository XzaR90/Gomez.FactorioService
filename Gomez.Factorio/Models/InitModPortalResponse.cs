using System.Text.Json.Serialization;

namespace Gomez.Factorio.Models
{
    public class InitModPortalResponse
    {
        [JsonPropertyName("upload_url")]
        public string UploadUrl { get; set; }
    }
}
