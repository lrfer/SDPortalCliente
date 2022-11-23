# Portal cliente
## Documentação

## Compilação

- Necessário a versão do .net 6.0 tanto SDK quanto Run time https://dotnet.microsoft.com/en-us/download
- Para start a aplicação precisa-se do comando``` dotnet run + (porta que deseja que ela suba) + a porta do portal admin ex: dotnet run 52855 51855```  
- Através da aplicação no one drive só necessita digitar ```./WS.SERVER + PORTA + Porta Portal Admin``` nela NÃO É SE NECESSÁRIO NENHUM DOWNLOAD DO .NET


# Objetos

- Produto : {
  "produtctId": null,
  "name": null,
  "quantity": 0,
  "price": 0.0
}

- Cliente : {
  "clientId": null,
  "name": null
  "OrderId" :null,
}

- Pedido :{
  "orderId": null,
  "clientId": null,
  "name": null,
  "quantity": 0,
  "price": 0.0,
  "produtoId": null
}



# Operações
### Pedido
- criarPedido(CID)
--- criarPedido(string)
--- Retorno: Sucesso : Objeto pedido com pedido.orderId = 123 Erro: Cliente não encontrado ou Erro: Não foi possivel cadastrar pedido
--- Ex: criarPedido(123)

- modificarPedido(CID, (Hash do criar pedido) ,Nome Produto,Quantidade)
--- modificarCliente(string,string,string,int)
--- Retorno: Sucesso : "Pedido Atualizado com sucesso" Error: "Cliente não encontrado" Ou Error: "Pedido não encontrado"
--- Ex: modificarPedido(123,D7BD47A8-BAFE-4F8E-874A-EB90040DD2F5,Leite,1)

- listarPedido(CID, OID)
--- listarPedido(string,string)
--- Retorno:  Sucesso :"Objeto pedido"  Erro : "Cliente não existe",ou Error: "Pedido não encontrado"
--- Ex: recuperarCliente(123,D7BD47A8-BAFE-4F8E-874A-EB90040DD2F5)

- listarPedidos(CID)
--- listarPedidos(string)
--- Retorno:  Sucesso :"Lista com pedidos"  Erro : "Cliente não existe",ou Error: "Pedido não encontrado"
--- Ex: apagarCliente(123)

- apagarPedido(CID, OID)
--- listarPedidos(string,string)
--- Retorno:  Sucesso :" Pedido excluido com sucesso"  Erro : "Cliente não existe",ou Error: "Pedido não encontrado"
--- Ex: apagarCliente(123,D7BD47A8-BAFE-4F8E-874A-EB90040DD2F5)

## Testes
#### Utilizando o postman é possível realizar testes
- https://www.postman.com/
- Teste 1
--- ``` inserirCliente(123,Luiz) Na api Portal Admin``` 
---- Deve retornar "Cliente cadastrado com sucesso"
- Teste 2
---  ```criarPedido(123)```
--- Deve retornar Objeto com Pedido.OrderId = Hash
- Teste 3
--- ```inserirProduto(123,Leite,2.0,2) Na Api portal Admin``` -
- Teste 4
--- ```modificarPedido(123, Hash,Leite,1)```
--- Deve retornar Objeto PEDIDO
- Teste 5
--- ```listarPedido(123,HASH)```
--- Deve retornar Pedido.Name = Leite
- Teste 6
--  ```criarPedido(123) ```
--- Deve retornar Objeto pedido.OrderId = Hash1
- Teste 7
--- ``` listarPedidos(123) ```
---- Deve retornar Lista com pedidos com Hash1 e Hash
- Teste 8
--  ```apagarPedido(123, Hash1)```
--- Deve retornar "Pedido excluido com sucesso"
- Teste 9
--  ```listarPedidos(123)```
--- Deve retornar Lista com pedidos com Hash


# Arquitetura
Foi usado uma arquitetura em camadas aonde se tem a parte de Domain que fica as entites, a parte de application aonde fica a logica de negocio e a parte de repository para persistencia, além da inversão e injeção de depência para facilitar manipular classes.

Para a parte de cache foi usando uma Lista simples de objeto
