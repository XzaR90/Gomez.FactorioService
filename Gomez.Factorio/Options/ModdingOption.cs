namespace Gomez.Factorio.Options
{
    public class ModdingOption
    {
        public const string SectionName = "Modding";

        public string? ModFolder { get; set; }

        public string? ApiKey { get; set; }

        public string? ModPortalUrl { get; set; }

        public bool EnableUpload { get; set; }
    }
}
