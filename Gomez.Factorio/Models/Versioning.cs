namespace Gomez.Factorio.Models
{
    public record Versioning
    {
        public Versioning(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return;
            }

            var v = version.Split(".");
            Major = int.Parse(v[0]);
            Minor = int.Parse(v[1]);
            Patch = int.Parse(v[2]);
        }

        public Versioning(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; init; }

        public int Minor { get; init; }

        public int Patch { get; init; }

        public Versioning Bump()
        {
            var (patch, minor, major) = this;

            patch++;
            if (patch > 999)
            {
                patch = 0;
                minor++;
            }

            if (minor > 999)
            {
                minor = 0;
                major++;
            }

            return new Versioning(major, minor, patch);
        }

        public void Deconstruct(out int patch, out int minor, out int major)
        {
            patch = Patch;
            minor = Minor;
            major = Major;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }
}
