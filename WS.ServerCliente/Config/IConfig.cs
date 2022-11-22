namespace Server.Config
{
    public interface IConfig
    {
        Task<string> TratarMensagem(string mensagem);
    }
}
