using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Server.Config;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace StartupService
{
    public class Startup : IHostedService
    {
        static HttpListener httpListener = new HttpListener();
        private static Mutex signal = new Mutex();
        private readonly IConfig config;
        private readonly string port;
        public Startup(IConfig config, string port)
        {
            this.config = config;
            this.port = port;
        }

        public async Task ReceiveConnection(HttpListenerContext context)
        {
            Console.WriteLine($"Cliente Conectado");
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                while (webSocket.State == WebSocketState.Open)
                {
                    string msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                    var resultado = await config.TratarMensagem(msg);
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{resultado}")), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
                    Console.WriteLine("Cliente desconectado");
                }
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var conectionString = $"http://localhost:{port}/";
            httpListener.Prefixes.Add(conectionString);
            httpListener.Start();
            await Listen(httpListener);

        }


        public async Task Listen(HttpListener listener)
        {
            while (true)
            {
                var context = listener.GetContext();
                Thread backgroundThread = new Thread(() => ReceiveConnection(context));
                backgroundThread.Start();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
