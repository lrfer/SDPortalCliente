namespace Domain.Entities
{
    public class Pedido
    {
        public string OrderId { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ProdutoId { get; set; }
    }
}
