using System.Text.Json.Serialization;

namespace Gomez.Factorio.Models
{
    public class ModInfo
    {
        public ModInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Homepage { get; set; }

        public string? FactorioVersion { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public Versioning Versioning => new(Version);
    }
}
