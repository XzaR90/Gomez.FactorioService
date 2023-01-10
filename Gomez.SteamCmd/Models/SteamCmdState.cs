namespace Gomez.SteamCmd.Models
{
    public record SteamCmdState
    {
        public string? CurrentOutput { get; init; } = null;

        public bool HasErrors { get; init; } = false;
    }
}
