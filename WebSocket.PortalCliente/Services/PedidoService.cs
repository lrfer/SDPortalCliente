using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IClienteRepository clienteRepository, 
                            IProdutoRepository produtoRepository)
        {
            this._pedidoRepository = pedidoRepository;
            this._clienteRepository = clienteRepository;
            this._produtoRepository = produtoRepository;
        }

        public async Task<string> CriarPedido(string clientId)
        {
            var result = await _clienteRepository.BuscarCliente(clientId);

            if (result == null)
                return "Cliente não encontrado";

            var pedido = new Pedido()
            {
                ClientId = clientId
            };

            return await _pedidoRepository.CriarPedido(pedido);
        }

        public async Task<string> ModificarPedido(Pedido pedido)
        {
            var result = await _clienteRepository.BuscarCliente(pedido.ClientId);
            if (result == null)
                return "Cliente não encontrado";

            var lstProduto = await _produtoRepository.RecuperarProdutos();
            if (lstProduto is null || !lstProduto.Any())
                return "Produto não encontrado";

            var produto = lstProduto.FirstOrDefault(x => x.Name == pedido.Name);

            if(produto is null)
                return "Produto não encontrado";

            if (pedido.Quantity == 0)
            {
                var ped = await ListarPedido(pedido.ClientId, pedido.OrderId);
                Produto prdAntigo = new Produto() { ProdutctId = ped.ProdutoId, Quantity = ped.Quantity };

                await ModificarProduto(prdAntigo, true);

                pedido.ProdutoId = string.Empty;
                pedido.Quantity = 0;
                pedido.Name = string.Empty;

                var repoPedido = await _pedidoRepository.ModificarPedido(pedido);

                if (repoPedido == 1)
                    return $"Pedido Atualizado com sucesso";
                else
                    return "Erro ao atualizar pedido";

            }
                

            if (produto.Quantity < pedido.Quantity)
                return "Quantidade não disponivel";

            pedido.ProdutoId = produto.ProdutctId;
            Produto prd = new Produto() { ProdutctId = pedido.ProdutoId, Quantity = pedido.Quantity};

            await ModificarProduto(prd);

            var repo = await _pedidoRepository.ModificarPedido(pedido);

            if (repo == 1)
                return $"Pedido Atualizado com sucesso";
            else
                return "Erro ao atualizar pedido";

        }

        public async Task<Pedido> ListarPedido(string clientId, string orderId)
        {
            var result = await _clienteRepository.BuscarCliente(clientId);
            if (result == null)
                return null;
            var repo = await _pedidoRepository.ListarPedido(clientId, orderId);

            if (repo != null)
                return repo;
            else
                return null;
        }


        public async Task<List<Pedido>> ListarPedidos(string clientId)
        {
            var result = await _clienteRepository.BuscarCliente(clientId);
            if (result == null)
                return null;
            return await _pedidoRepository.ListarPedidos(clientId);


        }
        public async Task ModificarPedidoQueue(Pedido pedido)
        {
            await _pedidoRepository.ModificarPedido(pedido);
        }

        public async Task<string> ApagarPedido(string clientId, string orderId)
        {
            var result = await _clienteRepository.BuscarCliente(clientId);
            if (result == null)
                return "Cliente não encontrado";

            var pedido = new Pedido()
            {
                ClientId = clientId,
                OrderId = orderId
            };

            var repo = await  _pedidoRepository.ApagarPedido(pedido);

            if (repo == 1)
                return "Pedido apagado com sucesso";
            else
                return "Erro ao apagar pedido";
        }
        private async Task ModificarProduto( Produto produto, bool adicionar = false)
        {
            await _produtoRepository.ModificarProduto(produto);
            
        }


    }
}
