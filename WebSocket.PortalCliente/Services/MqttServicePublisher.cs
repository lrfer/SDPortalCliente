using MQTTnet;
using MQTTnet.Client;

namespace Application.Services
{
    public static class MqttServicePublisher
    {
        public static async Task MqttServiceSendMsg(string msg,string topico)
        {
            try
            {
                var mqttFactory = new MqttFactory();

                IMqttClient client = mqttFactory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                                  .WithClientId(Const.ClientId.ToString())
                                  .WithTcpServer(Const.ConnectionMqtt, Const.ConnectionMqttPort)
                                  .WithCleanSession()
                                  .Build();

                await client.ConnectAsync(options);

                await PublishMessageAsync(client, msg,topico);

                await client.DisconnectAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private static async Task PublishMessageAsync(IMqttClient client,string mensagem,string topico)
        {
            var msg = new MqttApplicationMessageBuilder()
                        .WithTopic(topico)
                        .WithPayload(mensagem)
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                        .Build();
            if (client.IsConnected)
                await client.PublishAsync(msg);


        }
    }
}
