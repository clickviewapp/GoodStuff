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

        private TempDir(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var tempPath = Path.Combine(path, CreateRandomString());
            Debug.WriteLine("Creating temp directory: " + tempPath);
            Directory.CreateDirectory(tempPath);

            _path = tempPath;
        }

        /// <summary>
        /// Generates a new path in the current TempDir path directory
        /// </summary>
        /// <returns></returns>
        public string GenerateTempFilePath() => Path.Combine(_path, CreateRandomString());

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

        private static string CreateRandomString() => Guid.NewGuid().ToString("N");
    }
}