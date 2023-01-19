namespace Gomez.Factorio.Models
{
    public record Versioning
    {
        private const int VersionMax = 999;

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
            if (patch > VersionMax)
            {
                patch = 0;
                minor++;
            }

            if (minor > VersionMax)
            {
                minor = 0;
                major++;
            }

            return new Versioning(major, minor, patch);
        }

        public Versioning Debump()
        {
            var (patch, minor, major) = this;

            patch--;
            if (patch < 0)
            {
                patch = VersionMax;
                minor--;
            }

            if (minor < 0)
            {
                minor = VersionMax;
                major--;
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
