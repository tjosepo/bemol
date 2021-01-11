using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;

using HttpMultipartParser;
using Bemol.Http.Util;

namespace Bemol.Http {
    internal class Form {
        internal readonly NameValueCollection Parameters;
        internal readonly Dictionary<string, UploadedFile> Files;

        internal Form(Context ctx) {
            (Parameters, Files) = (ctx.IsMultipartFormData())
                    ? MultipartFormData(new MemoryStream(ctx.BodyAsBytes()))
                    : (ContextUtil.SplitKeyValueString(ctx.Body()), new Dictionary<string, UploadedFile>());
        }

        /// TODO: Use streaming instead of parsing everything in one go.
        ///       Without streaming, it still works with files >1GB.
        private (NameValueCollection, Dictionary<string, UploadedFile>) MultipartFormData(Stream stream) {
            var parameters = new NameValueCollection();
            var files = new Dictionary<string, UploadedFile>();
            var parser = MultipartFormDataParser.Parse(stream);

            foreach (var param in parser.Parameters) {
                parameters.Add(param.Name, param.Data);
            }

            foreach (var file in parser.Files) {
                files.Add(file.Name, new UploadedFile(
                        file.Data,
                        file.ContentType,
                        file.Data.Length,
                        file.FileName,
                        Path.GetExtension(file.FileName)));
            }

            return (parameters, files);
        }
    }
}