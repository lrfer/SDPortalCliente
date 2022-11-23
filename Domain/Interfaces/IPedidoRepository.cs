using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPedidoRepository
    {
        Task<int> ModificarPedido(Pedido pedido);
        Task<Pedido> ListarPedido(string clientId, string OrderId);
        Task<List<Pedido>> ListarPedidos(string clientId);
        Task<int> ApagarPedido(Pedido pedido);
        Task<string> CriarPedido(Pedido pedido);
        Task DeletarLocalDoBanco(Pedido pedido);
    }
}
