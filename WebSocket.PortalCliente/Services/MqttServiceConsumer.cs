using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Client;

using System.Text;
using System.Text.Json;

namespace Application.Services
{
    public class MqttServiceConsumer : BackgroundService, IDisposable
    {

        IMqttClient _client;

        MqttClientOptions _clientOptions;
        MqttClientSubscribeOptions _subscriptionOptions;
        private readonly IPedidoService _pedidoService;

        public MqttServiceConsumer(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
            var mqttFactory = new MqttFactory();

            _client = mqttFactory.CreateMqttClient();

            _clientOptions = new MqttClientOptionsBuilder()
                              .WithClientId(Const.ClientId.ToString())
                              .WithTcpServer(Const.ConnectionMqtt, Const.ConnectionMqttPort)
                              .Build();



            _subscriptionOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                     .WithTopicFilter(f =>
                     {
                         f.WithTopic(Const.QueuePedidos);

                     })
                     .WithTopicFilter(f => 
                     {
                         f.WithTopic(Const.QueuePedidoRemover);
                     }
                     )
                     .Build();


            _client.ApplicationMessageReceivedAsync += HandleMessageAsync;
        }



        async Task HandleMessageAsync(MqttApplicationMessageReceivedEventArgs e)
        {

            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            if (e.ApplicationMessage.Topic.Equals(Const.QueuePedidos))
            {
                var result = JsonSerializer.Deserialize<Pedido>(payload);
                if (result is not null)
                    await _pedidoService.ModificarPedidoQueue(result);
            }
            if (e.ApplicationMessage.Topic.Equals(Const.QueuePedidos))
            {
                var result = JsonSerializer.Deserialize<Pedido>(payload);
                if (result is not null)
                    await _pedidoService.SicronizarDelete(result);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _client.ConnectAsync(_clientOptions, CancellationToken.None);

            await _client.SubscribeAsync(_subscriptionOptions, CancellationToken.None);
        }
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
            await base.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client.Dispose();
                base.Dispose();
            }
            _client = null;
        }
    }
}
