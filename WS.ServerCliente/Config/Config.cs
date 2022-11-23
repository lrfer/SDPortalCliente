using Application.Services.Interfaces;
using Domain.Entities;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Server.Config
{
    public class Config : IConfig
    {
        private readonly IPedidoService pedidoService;

        public Config(IPedidoService pedidoService)
        {
            this.pedidoService = pedidoService;
        }

        public async Task<string> TratarMensagem(string mensagem)
        {
            try
            {
                if (mensagem.Contains("criarPedido"))
                {
                    var itens = mensagem.Split("(");
                    var values = itens[1].Split((","));
                    var cid = RemoverChars(values[0]);

                    return await pedidoService.CriarPedido(cid);
                }

                if (mensagem.Contains("modificarPedido"))
                {
                    var itens = mensagem.Split("(");
                    var values = itens[1].Split((","));
                    var cid = RemoverChars(values[0]);
                    var OID = RemoverChars(values[1]);
                    var produtoNome = RemoverChars(values[2]);
                    var quantidade = Int32.Parse(RemoverChars(values[3]));

                    var pedido = new Pedido
                    {
                        ClientId = cid,
                        OrderId = OID,
                        Name = produtoNome,
                        Quantity = quantidade

                    };
                    return await pedidoService.ModificarPedido(pedido);
                }

                if (mensagem.Contains("listarPedido"))
                {
                    var itens = mensagem.Split("(");
                    var values = itens[1].Split((","));
                    var cid = RemoverChars(values[0]);
                    string oid = string.Empty;
                    if (values.Length > 1)
                    {
                        oid = RemoverChars(values[1]);
                        var retorno = await pedidoService.ListarPedido(cid, oid);
                        return JsonSerializer.Serialize(retorno);
                    }

                    var result = await pedidoService.ListarPedidos(cid);

                    if (result == null)
                        return ("Não existe pedido");
                    else
                        return (JsonSerializer.Serialize(result));
                }

                if (mensagem.Contains("apagarPedido"))
                {
                    var itens = mensagem.Split("(");
                    var values = itens[1].Split((","));
                    var cid = RemoverChars(values[0]);
                    var oid = RemoverChars(values[1]);

                    return  await pedidoService.ApagarPedido(cid,oid);
                }


                else
                    return "Operação não cadastrada";
            }
            catch(Exception ex)
            {
                Console.WriteLine("Erro ao processar requisição");
                return "Erro ao processar requisiçã";
            }
        }


        private string RemoverChars(string str)
        {
            var charsToRemove = new string[] { "(", ")", "\n", "\r" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty).Trim().ToUpper();
            }
            return str;
        }
    }
}
