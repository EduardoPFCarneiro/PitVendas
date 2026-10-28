namespace PitVendasApi.Services
{
    public class RegraNegocioException : Exception
    {
        public RegraNegocioException(string mensagem) : base(mensagem) { }
    }
}
