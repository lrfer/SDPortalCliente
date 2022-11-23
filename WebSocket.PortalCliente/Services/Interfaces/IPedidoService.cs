using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<string> CriarPedido(string clientId);
        Task<string> ModificarPedido(Pedido pedido);
        Task<Pedido> ListarPedido(string clientId, string orderId);
        Task<List<Pedido>> ListarPedidos(string clientId);

        Task<string> ApagarPedido(string clientId, string orderId);

        Task ModificarPedidoQueue(Pedido pedido);
        Task SicronizarDelete(Pedido pedido);

    }
}
