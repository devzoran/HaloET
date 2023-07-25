using System.Collections.Generic;

namespace FUIEditor
{
    public class PackageInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public Dictionary<string, PackageComponentInfo> PackageComponentInfos { get; } = new();
    }
}