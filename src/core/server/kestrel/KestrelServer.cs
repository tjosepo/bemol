// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Hosting;
// using Microsoft.AspNetCore.Hosting.Server;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Http.Features;
// using Microsoft.AspNetCore.Server.Kestrel.Core;
// using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
// using Microsoft.Extensions.Logging.Abstractions;
// using Microsoft.Extensions.Options;

// using Bemol.Http;

// namespace Bemol.Core.Server.Kestrel {

//     internal class KServer : BemolServer {
//         private BemolConfig Config;

//         internal KServer(BemolConfig config) : base(config) {
//             Config = config;
//         }

//         internal override void Start() {
//             new Thread(async () => {
//                 var options = SetServerOptions();
//                 var transportOptions = new SocketTransportOptions();
//                 var logger = new NullLoggerFactory();
//                 var applicationLifetime = new ApplicationLifetime();

//                 var transport = new SocketTransportFactory(
//                 new OptionsWrapper<SocketTransportOptions>(transportOptions), logger);

//                 var server = new KestrelServer(options, transport, logger);
//                 await server.StartAsync(new Application(this), CancellationToken.None);

//                 Console.Clear();
//                 Console.WriteLine($"Listening on http://{Host}:{Port}{Config.ContextPath}");

//                 while (Started) Thread.Yield();
//                 server.Dispose();
//             }).Start();
//         }

//         private OptionsWrapper<KestrelServerOptions> SetServerOptions() {
//             var options = new KestrelServerOptions();
//             options.ListenLocalhost(Port, listenOptions => {
//                 listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
//             });
//             options.AllowSynchronousIO = true;

//             return new OptionsWrapper<KestrelServerOptions>(options);
//         }
//     }

//     class ApplicationLifetime : IHostApplicationLifetime {
//         private readonly CancellationTokenSource startedSource = new CancellationTokenSource();
//         private readonly CancellationTokenSource stoppingSource = new CancellationTokenSource();
//         private readonly CancellationTokenSource stoppedSource = new CancellationTokenSource();

//         public CancellationToken ApplicationStarted => startedSource.Token;

//         public CancellationToken ApplicationStopping => stoppingSource.Token;

//         public CancellationToken ApplicationStopped => stoppedSource.Token;

//         public void StopApplication() {
//             lock (stoppingSource) {
//                 if (!stoppingSource.Token.IsCancellationRequested)
//                     stoppingSource.Cancel(throwOnFirstException: false);
//             }
//         }
//     }

//     class Application : IHttpApplication<Context> {
//         private readonly BemolServer Server;

//         public Application(BemolServer server) => Server = server;

//         public Context CreateContext(IFeatureCollection features) {
//             HttpContext httpContext = new DefaultHttpContext(features);
//             return new KestrelContext(httpContext, new BemolConfig());
//         }

//         public void DisposeContext(Context context, Exception exception) { }

//         public async Task ProcessRequestAsync(Context ctx) => await Task.Run(() => Server.HandleRequest(ctx));
//     }
// }