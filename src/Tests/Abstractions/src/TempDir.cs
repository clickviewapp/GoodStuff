namespace ClickView.GoodStuff.Tests.Abstractions
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class TempDir : IDisposable
    {
        private readonly string _path;

        public TempDir() : this(Path.GetTempPath())
        {
        }

        public TempDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var tempPath = GenerateTempFilePath(path);
            Debug.WriteLine("Creating temp directory: " + tempPath);
            Directory.CreateDirectory(tempPath);

            _path = tempPath;
        }

        protected string GenerateTempFilePath(string path) => Path.Combine(path, Guid.NewGuid().ToString("N"));

        public void Dispose()
        {
            Debug.WriteLine("Deleting temp directory: " + _path);
            Directory.Delete(_path, true);
        }

        public override string ToString()
        {
            return _path;
        }

        public static implicit operator string(TempDir tempDir) => tempDir._path;
    }
}