using System;
using System.IO;

using MimeTypes;
using Bemol.Core;
using Bemol.Http.Exceptions;

namespace Bemol.Http {
    internal class StaticFilesHandler {
        private readonly BemolConfig Config;

        internal StaticFilesHandler(BemolConfig config) {
            Config = config;
        }

        internal void Handle(Context ctx) {
            try {
                var path = ctx.Path();
                var target = GetTargetPath(path);
                if (!File.Exists(target)) {
                    ctx.Status(404);
                    return;
                }

                var bytes = File.ReadAllBytes(target);
                var extension = Path.GetExtension(target);
                ctx.Result(bytes)
                    .Status(200)
                    .ContentType(GetContentType(extension));
            } catch (Exception) {
                throw new NotFoundException("Exception occurred while handling static resource.");
            }
        }

        internal string GetTargetPath(string path) {
            var currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var staticFolder = BemolUtil.NormalizePath(Config.StaticFolder);
            var requestPath = BemolUtil.NormalizePath(path);
            var separator = Path.DirectorySeparatorChar;

            if (!requestPath.Contains('.')) requestPath += $"{separator}index.html";
            return $"{currentDirectory}{staticFolder}{separator}{requestPath}";
        }

        internal string GetContentType(string extension) => MimeTypeMap.GetMimeType(extension);
    }
}
