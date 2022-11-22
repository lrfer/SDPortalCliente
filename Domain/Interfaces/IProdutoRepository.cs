using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> RecuperarProdutos();
        Task<int> ModificarProduto(Produto produto, bool adicionar = false);
    }
}
