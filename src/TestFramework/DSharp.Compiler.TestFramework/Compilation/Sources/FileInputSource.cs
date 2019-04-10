using System;
using System.IO;

namespace DSharp.Compiler.TestFramework.Compilation.Sources
{
    public class FileInputSource : IStreamSource
    {
        private readonly FileInfo fileInfo;
        private Stream openStream;

        public string FullName
        {
            get { return fileInfo?.FullName; }
        }

        public string Name
        {
            get { return fileInfo?.Name; }
        }

        public FileInputSource(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("File Stream not found", filePath);
            }
        }

        public Stream GetStream()
        {
            return fileInfo.OpenRead();
        }
    }
}
