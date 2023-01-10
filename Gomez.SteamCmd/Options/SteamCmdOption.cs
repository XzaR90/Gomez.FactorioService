namespace Gomez.SteamCmd.Options
{
    public class SteamCmdOption
    {
        public const string SectionName = "Steam";

        public int AppId { get; set; } = default;

        public string AppPath { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string CmdPath { get; set; } = string.Empty;
    }
}
