using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PitVendasApi.Services
{
    public class TratadorDeErros : IExceptionHandler
    {
        private readonly ILogger<TratadorDeErros> _logger;

        public TratadorDeErros(ILogger<TratadorDeErros> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception excecao, CancellationToken ct)
        {
            var problema = excecao switch
            {
                RegraNegocioException regra => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Regra de negocio",
                    Detail = regra.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erro interno",
                    Detail = "Ocorreu um erro inesperado ao processar a requisicao."
                }
            };

            if (problema.Status == StatusCodes.Status500InternalServerError)
                _logger.LogError(excecao, "Erro nao tratado em {Caminho}", context.Request.Path);

            context.Response.StatusCode = problema.Status!.Value;
            await context.Response.WriteAsJsonAsync(problema, ct);

            return true;
        }
    }
}
