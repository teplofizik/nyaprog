using System;
using System.IO;

using Programmer.Platform;

namespace Programmer
{
    /// <summary>
    /// Реализация IFileSystem поверх System.IO.
    /// </summary>
    public sealed class FileSystem : IFileSystem
    {
        public bool Exists(string Path) => File.Exists(Path);

        public long GetLength(string Path) => new FileInfo(Path).Length;

        public DateTime GetLastWriteTimeUtc(string Path) => new FileInfo(Path).LastWriteTimeUtc;

        public string ReadAllText(string Path) => File.ReadAllText(Path);

        public void WriteAllText(string Path, string Text) => File.WriteAllText(Path, Text);

        public byte[] ReadAllBytes(string Path) => File.ReadAllBytes(Path);

        public void WriteAllBytes(string Path, byte[] Data) => File.WriteAllBytes(Path, Data);

        public void Delete(string Path) => File.Delete(Path);

        public void Copy(string Source, string Dest, bool Overwrite = false) => File.Copy(Source, Dest, Overwrite);

        public void Move(string Source, string Dest, bool Overwrite = false)
        {
            if (Overwrite && File.Exists(Dest)) File.Delete(Dest);
            File.Move(Source, Dest);
        }
    }
}
