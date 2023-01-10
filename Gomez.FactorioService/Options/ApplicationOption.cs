namespace Gomez.FactorioService.Options
{
    public class GameOption
    {
        public const string SectionName = "Game";

        public string ExePath { get; set; } = string.Empty;

        public string SavePath { get; set; } = string.Empty;

        public string SettingsPath { get; set; } = string.Empty;
    }
}
