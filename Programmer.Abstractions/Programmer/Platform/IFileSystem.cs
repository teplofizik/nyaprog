using System;

namespace Programmer.Platform
{
    /// <summary>
    /// Файловые операции без привязки к конкретной ОС/реализации.
    /// </summary>
    public interface IFileSystem
    {
        bool Exists(string Path);

        long GetLength(string Path);

        DateTime GetLastWriteTimeUtc(string Path);

        string ReadAllText(string Path);

        void WriteAllText(string Path, string Text);

        byte[] ReadAllBytes(string Path);

        void WriteAllBytes(string Path, byte[] Data);

        void Delete(string Path);

        void Copy(string Source, string Dest, bool Overwrite = false);

        void Move(string Source, string Dest, bool Overwrite = false);
    }
}
