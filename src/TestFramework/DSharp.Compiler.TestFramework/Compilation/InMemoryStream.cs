using System;
using System.IO;
using System.Text;

namespace DSharp.Compiler.TestFramework.Compilation
{
    public sealed class InMemoryStream : IStreamSource
    {
        private readonly string name;

        public string GeneratedOutput { get; private set; }

        string IStreamSource.FullName
        {
            get { return name; }
        }

        string IStreamSource.Name
        {
            get { return name; }
        }

        public InMemoryStream() => name = Guid.NewGuid().ToString();

        public Stream GetStream()
        {
            return new NotifiableMemoryStream
            {
                OnBeforeDispose = HandleOnDisposed
            };
        }

        private void HandleOnDisposed(byte[] buffer, long length)
        {
            GeneratedOutput = Encoding.UTF8.GetString(buffer, 0, (int)length);
        }

        private class NotifiableMemoryStream : MemoryStream
        {
            public Action<byte[], long> OnBeforeDispose { get; set; }

            protected override void Dispose(bool disposing)
            {
                var buffer = GetBuffer();
                OnBeforeDispose?.Invoke(buffer, Length);
                base.Dispose(disposing);
            }
        }
    }
}
