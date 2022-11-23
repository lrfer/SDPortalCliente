using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        List<Pedido> pedidosCache;
        List<Pedido> pedidosBanco;

        public PedidoRepository()
        {
            this.pedidosCache = new();
            this.pedidosBanco = new();
        }

        public async Task<int> ApagarPedido(Pedido pedido)
        {
            var ped = await ListarPedido(pedido.ClientId, pedido.OrderId);
              if(ped is not null)
            {
                pedidosCache.Remove(ped);
                await EnviarDelete(ped);
                return 1;
            }
            else
                return 0;
        }

        public async Task<string> CriarPedido(Pedido pedido)
        {
            var orderId = Guid.NewGuid().ToString().ToUpper();
            var pedidoBanco = await ListarPedido(pedido.ClientId,orderId);
            if (pedidoBanco is null)
            {
                pedido.OrderId = orderId;
                pedidosCache.Add(pedido);
                await SalvarNoBanco(pedido);
                return orderId;
            }
            else return "Pedido já cadastrado";
        }

        public async Task<Pedido> ListarPedido(string clientId, string OrderId)
        {
            var pedidoLocal = pedidosCache.FirstOrDefault(x => x.ClientId == clientId && x.OrderId == OrderId);
            var pedidoBanco = pedidosBanco.FirstOrDefault(x => x.ClientId == clientId && x.OrderId == OrderId);
            if (pedidoLocal is null)
            {
                if (pedidoBanco is null)
                    return null;
                else
                {
                    SyncBanco();
                    return pedidoBanco;
                }

            }
            else if(pedidoBanco is not null)
            {
                if(pedidoLocal.ProdutoId == pedidoBanco.ProdutoId)
                    return pedidoLocal;
                else{
                        SyncBanco();
                        return pedidoBanco;
                    }
            }
            else
                return pedidoLocal;
        }

        public async Task<List<Pedido>> ListarPedidos(string clientId)
        {
            return pedidosCache;
        }

        public async Task<int> ModificarPedido(Pedido pedido)
        {
            var ped = await ListarPedido(pedido.ClientId, pedido.OrderId);
            if (ped is not null)
            {
                ped.ClientId = pedido.ClientId;
                ped.OrderId = pedido.OrderId;
                ped.Name = pedido.Name;
                ped.ProdutoId = pedido.ProdutoId;
                ped.Quantity = pedido.Quantity;
                ped.Price = pedido.Price;

                await SalvarNoBanco(ped);
                return 1;

            }
            else
            {
                pedidosCache.Add(pedido);
                return 0;
            }
            
        }
        public async Task SalvarDoBanco(Pedido pedido)
        {
            var pedidoLocal = pedidosBanco.FirstOrDefault(x => x.ClientId == pedido.ClientId && x.OrderId == pedido.OrderId);
            if (pedidoLocal is null)
                pedidosBanco.Add(pedidoLocal);
            else
            {
                pedidoLocal.Name = pedido.Name;
                pedidoLocal.Price = pedido.Price;
                pedidoLocal.Quantity = pedido.Quantity;
            }
        }

        public async Task DeletarLocalDoBanco(Pedido pedido)
        {
            var pedidoLocal = pedidosCache.FirstOrDefault(x => x.ClientId == pedido.ClientId && x.OrderId == pedido.OrderId);
            if (pedidoLocal is not null)
            {
                pedidosCache.Remove(pedidoLocal);
                pedidosBanco.Remove(pedidoLocal);
            }

        }

        #region "Metodos privados"
        private void SyncBanco()
        {
            pedidosCache = new();
            foreach (var produto in pedidosBanco)
                pedidosCache.Add(produto);
        }
        private async Task SalvarNoBanco(Pedido pedido)
        {
            string json = JsonSerializer.Serialize(pedido);
            try
            {
                await MqttServicePublisher.MqttServiceSendMsg(json, Const.QueuePedidos);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private async Task EnviarDelete(Pedido pedido)
        {
            string json = JsonSerializer.Serialize(pedido);
            await MqttServicePublisher.MqttServiceSendMsg(json, Const.QueuePedidoRemover);
        }

        #endregion
    }
}
