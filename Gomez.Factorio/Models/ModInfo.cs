using System.Text.Json.Serialization;

namespace Gomez.Factorio.Models
{
    public class ModInfo
    {
        public ModInfo(string name, string version, string factorioVersion)
        {
            Name = name;
            Version = version;
            FactorioVersion = factorioVersion;
        }

        public string Name { get; set; }

        public string Version { get; set; }

        public string? Title { get; set; }

        public string? Author { get; set; }

        public string? Homepage { get; set; }

        [JsonPropertyName("factorio_version")]
        public string FactorioVersion { get; set; }

        public string[]? Dependencies { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public Versioning Versioning => new(Version);
    }
}
