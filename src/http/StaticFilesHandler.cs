using System;
using System.IO;
using Bemol.Core;
using Bemol.Http.Exceptions;
using MimeTypes;

namespace Bemol.Http {
    public class StaticFilesHandler {

        BemolConfig Config;

        public StaticFilesHandler(BemolConfig config) {
            Config = config;
        }

        public void Handle(Context ctx) {
            try {
                var path = ctx.Path();
                if (path == "/") path = "/index.html";

                var target = GetTargetPath(path);
                if (!File.Exists(target)) return;

                var bytes = File.ReadAllBytes(target);
                var extension = Path.GetExtension(target);
                ctx.Result(bytes)
                    .Status(200)
                    .ContentType(GetContentType(extension));
            } catch (Exception) {
                throw new NotFoundException("Exception occurred while handling static resource.");
            }
        }

        private string GetTargetPath(string path) {
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var publicFolder = BemolUtil.NormalizePath(Config.PublicFolder);
            var requestPath = BemolUtil.NormalizePath(path);
            var separator = Path.DirectorySeparatorChar;
            return $"{currentDirectory}{publicFolder}{separator}{requestPath}";
        }

        private string GetContentType(string extension) {
            return MimeTypeMap.GetMimeType(extension);
        }
    }
}
