using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        IVariable variable;

        public ClienteRepository(IVariable variable)
        {
            this.variable = variable;
        }

        public async Task<Cliente> BuscarCliente(string clientid)
        {
            Console.WriteLine("Buscando cliente");
            Cliente clienteApi = new();
            using (ClientWebSocket cliente = new())
            {
                Uri serviceUri = new Uri($"ws://localhost:{variable.GetServerIp()}/");
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));
                try
                {
                    await cliente.ConnectAsync(serviceUri, cTs.Token);
                    var msg = $"recuperarCliente({clientid})";
                    while (cliente.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
                        await cliente.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);
                        var responseBuffer = new byte[1024];
                        var offset = 0;
                        var packet = 1024;
                        while (true)
                        {
                            ArraySegment<byte> byteRecieved = new ArraySegment<byte>(responseBuffer, offset, packet);
                            WebSocketReceiveResult response = await cliente.ReceiveAsync(byteRecieved, cTs.Token);
                            var responseMsg = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                            if (response.EndOfMessage)
                            {
                                if (responseMsg is not null)
                                {
                                    clienteApi = JsonSerializer.Deserialize<Cliente>(responseMsg);
                                    await cliente.CloseOutputAsync(WebSocketCloseStatus.NormalClosure,null,cTs.Token);
                                    return clienteApi;
                                }

                                break;
                            }
                        }

                    }

                }


                catch (Exception ex)
                {
                    return null;
                }

                return clienteApi;
            }
        }
    }
}
