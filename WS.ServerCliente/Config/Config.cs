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

                //if (mensagem.Contains("recuperarCliente"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var cid = RemoverChars(values[0]);

                //    var result = await clienteService.BuscarCliente(cid);

                //    if (result == null)
                //        return ("Cliente não existe");
                //    else
                //        return (JsonSerializer.Serialize(result));
                //}

                //if (mensagem.Contains("apagarCliente"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var cid = RemoverChars(values[0]);

                //    var result = await clienteService.ApagarCliente(cid);
                //    if (result == 0)
                //        return ("Cliente não existe");
                //    else
                //        return ("Cliente apagado com sucesso");
                //}

                //if (mensagem.Contains("inserirProduto"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var Pid = values[0];
                //    var nome = RemoverChars(values[1]);
                //    decimal preco = decimal.Parse(RemoverChars(values[2]), CultureInfo.InvariantCulture);
                //    int quantidade = 0;
                //    int.TryParse(RemoverChars(values[3]), out quantidade);

                //    var produto = new Produto
                //    {
                //        ProdutctId = Pid,
                //        Name = nome,
                //        Price = preco,
                //        Quantity = quantidade

                //    };

                //    var result = await produtoService.InserirProduto(produto);

                //    if (result == 0)
                //        return ("Produto já existe");
                //    else
                //        return ("Produto cadastrado com sucesso");
                //}


                //if (mensagem.Contains("modificarProduto"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var Pid = values[0];
                //    var nome = RemoverChars(values[1]);
                //    decimal preco = decimal.Parse(RemoverChars(values[2]), CultureInfo.InvariantCulture);
                //    int quantidade = 0;
                //    int.TryParse(RemoverChars(values[3]), out quantidade);

                //    var produto = new Produto
                //    {
                //        ProdutctId = Pid,
                //        Name = nome,
                //        Price = preco,
                //        Quantity = quantidade

                //    };

                //    var result = await produtoService.AtualizarProduto(produto);

                //    if (result == 0)
                //        return ("Produto não existe");
                //    else
                //        return ("Produto atualizado com sucesso");
                //}

                //if (mensagem.Contains("recuperarProduto"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var pid = RemoverChars(values[0]);

                //    var result = await produtoService.BuscarProduto(pid);


                //    if (result == null)
                //        return ("Produto não existe");
                //    else
                //        return (JsonSerializer.Serialize(result));
                //}

                //if (mensagem.Contains("apagarProduto"))
                //{
                //    var itens = mensagem.Split("(");
                //    var values = itens[1].Split((","));
                //    var pid = RemoverChars(values[0]);

                //    var result = await produtoService.ApagarProduto(pid);
                //    if (result == 0)
                //        return ("Produto não existe");
                //    else
                //        return ("Produto apagado com sucesso");
                //}

                else
                    return "Operação não cadastrada";
            }
            catch(Exception ex)
            {
                Console.WriteLine("Erro ao processar requisição");
                return "a";
            }
        }


        private string RemoverChars(string str)
        {
            var charsToRemove = new string[] { "(", ")", "\n", "\r" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty);
            }
            return str;
        }
    }
}
