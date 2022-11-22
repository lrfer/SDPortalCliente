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

        public Task<int> ApagarPedido(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CriarPedido(Pedido pedido)
        {
            var orderId = Guid.NewGuid().ToString();
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
            if (pedidoLocal is null)
            {
                var pedidoBanco = pedidosBanco.FirstOrDefault(x => x.ClientId == clientId && x.OrderId == OrderId);
                if (pedidoBanco is null)
                    return null;
                else
                {
                    SyncBanco();
                    return pedidoBanco;
                }

            }
            else
                return pedidoLocal;
        }

        public Task<List<Pedido>> ListarPedidos(string clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ModificarPedido(Pedido pedido)
        {
            var ped = ListarPedido(pedido.ClientId, pedido.OrderId);
            if (ped is not null)
            {
                pedidosCache.FirstOrDefault(x => x.OrderId == pedido.OrderId);
                await SalvarNoBanco(pedido);
                return 1;

            }
            else
                return 0;
            
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
            await MqttServicePublisher.MqttServiceSendMsg(json, Const.QueuePedidos);
        }

        #endregion
    }
}
