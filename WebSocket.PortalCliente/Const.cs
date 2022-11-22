namespace Application
{
    public static class Const
    {
        public readonly static string ConnectionMqtt = "127.0.0.1";
        public readonly static string QueuePedidos= "PEDIDO/";
        public readonly static string QueueProdutoRemover = "PRODUTOS_REMOVER/";
        public readonly static int ConnectionMqttPort = 1883;
        public static readonly Guid ClientId = Guid.NewGuid();
    }
    public class Variables : IVariable
    {
        public string server;

        public Variables(string server)
        {
            this.server = server;
        }

        public string GetServerIp()
        {
            return server;
        }
    }
    public interface IVariable
    {
        string GetServerIp();
    }
}
