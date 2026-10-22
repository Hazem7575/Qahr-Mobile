using System.IO;
using System.IO.Compression;

namespace Qahr.Mobile.Role
{
    /// <summary>
    /// The server's map files (Data\map\map). A map is stored zlib-compressed as &lt;name&gt;.dmap.z (a DMap is
    /// ~170x smaller compressed: Data\map went from 1.1 GB to a few MB); a plain &lt;name&gt;.dmap is still read
    /// when there is one (a newly copied map). Every reader of a DMap goes through here.
    /// </summary>
    public static class MapFiles
    {
        /// <summary>mapFile = the GameMap.dat path with .dmap (map/map/x.dmap), relative to CO2Folder</summary>
        public static string FullPath(string mapFile)
        {
            return Path.Combine(Program.ServerConfig.CO2Folder, mapFile);
        }

        /// <summary>the file that holds the map (compressed first), or null</summary>
        public static FileInfo Find(string mapFile)
        {
            string p = FullPath(mapFile);
            var z = new FileInfo(p + ".z");
            if (z.Exists) return z;
            var raw = new FileInfo(p);
            return raw.Exists ? raw : null;
        }

        public static bool Exists(string mapFile)
        {
            return Find(mapFile) != null;
        }

        /// <summary>the DMap bytes (decompressed); throws FileNotFoundException when the map is missing</summary>
        public static byte[] Read(string mapFile)
        {
            var f = Find(mapFile);
            if (f == null)
                throw new FileNotFoundException("map file missing", FullPath(mapFile));
            if (!f.Name.EndsWith(".z"))
                return File.ReadAllBytes(f.FullName);
            using (var fs = f.OpenRead())
            using (var z = new ZLibStream(fs, CompressionMode.Decompress))
            using (var ms = new MemoryStream())
            {
                z.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
