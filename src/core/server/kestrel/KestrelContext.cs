using Microsoft.AspNetCore.Http;

using Bemol.Http;

namespace Bemol.Core.Server.Kestrel {
    public sealed class KestrelContext : Context {
        public KestrelContext(HttpContext context, BemolConfig config)
            : base(new KestrelRequest(context), new KestrelResponse(context), config) { }
    }
}