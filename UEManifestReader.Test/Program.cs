using System;
using System.Diagnostics;
using System.Linq;

using UEManifestReader.Enums;

namespace UEManifestReader.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            UESerializedManifestReader x = new(
                string.Empty,
                CustomManifestReadSettings.ReadOnlyWhatIsNecessaryForDownload,
                true,
                string.Empty,
                JsonOutputFormatFlags.Grouped | JsonOutputFormatFlags.Indented);

            Stopwatch a = new();
            a.Start();
            x.ReadManifest();
            a.Stop();
            long gameSize = x.Manifest.FileList.Sum(x => x.ChunkParts.Sum(x => x.Size));
            Console.WriteLine($"Parsed in: {a.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine($"App Name: {x.Manifest.ManifestMeta.AppName}");
            Console.WriteLine($"App Version: {x.Manifest.ManifestMeta.BuildVersion}");
            Console.WriteLine($"File Count: {x.Manifest.FileList.Count}");
            Console.WriteLine($"Chunks Count: {x.Manifest.ChunkList.Count}");
            Console.WriteLine($"App size {gameSize / 1.074e+9} GB");
            Console.WriteLine($"Example URL: {x.Manifest.BaseUrls[0]}/{x.Manifest.ChunkSubdir}/{x.Manifest.ChunkList[0]}");
        }
    }
}