namespace Gomez.Factorio.Models
{
    public static class FactorioPath
    {
        static FactorioPath()
        {
            ScriptOutput = Path.Combine(AppData, "script-output");
            Mods = Path.Combine(AppData, "mods");
        }

        public static string AppData { get; private set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Factorio");

        public static string ScriptOutput { get; set; }
        public static string Mods { get; set; }
    }
}
