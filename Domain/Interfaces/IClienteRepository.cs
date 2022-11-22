using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente> BuscarCliente(string clientid);
    }
}
