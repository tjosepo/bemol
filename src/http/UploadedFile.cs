using System.IO;

namespace Bemol.Http {
    public class UploadedFile {
        /// <summary> The file-content as an <c>FileStream<c> </summary>
        public readonly Stream Content;

        /// <summary> The content-type passed by the client </summary>
        public readonly string ContentType;

        /// <summary> The size of the file in bytes </summary>
        public readonly long Size;

        /// <summary> The file-name reported by the client </summary>
        public readonly string Filename;

        /// <summary> The file-extension, extracted from the filename </summary>
        public readonly string Extension;

        internal UploadedFile(Stream content, string contentType, long size, string filename, string extension) {
            Content = content;
            ContentType = contentType;
            Size = size;
            Filename = filename;
            Extension = extension;
        }
    }
}