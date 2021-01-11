namespace Bemol.Core {
    public class BemolConfig {
        public string ResourcesFolder = "/resources";
        public string PartialsFolder = "/partials";
        public string StaticFolder = "/public";
        public string ContextPath = "/";
        public string DefaultContentType = "text/plain";
        public bool IgnoreTrailingSlashes = true;
        public bool EnableCorsForAllOrigins = false;
    }
}