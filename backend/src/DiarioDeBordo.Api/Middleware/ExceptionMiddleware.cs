using FluentValidation;
using System.Text.Json;

namespace DiarioDeBordo.Api.Middleware;

/// <summary>
/// Middleware centralizado de tratamento de exceções.
/// Garante que stack traces e detalhes internos não sejam expostos em produção
/// (DevSecOps: sem exposição de dados sensíveis em respostas de erro).
/// </summary>
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Erro de validação: {Errors}", ex.Errors.Select(e => e.ErrorMessage));
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var erros = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                titulo = "Erro de validação",
                status = 400,
                erros
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado ao processar requisição {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var resposta = env.IsDevelopment()
                ? new { titulo = "Erro interno", status = 500, detalhe = ex.Message }
                : new { titulo = "Erro interno do servidor", status = 500, detalhe = "Contate o suporte." };

            await context.Response.WriteAsync(JsonSerializer.Serialize(resposta));
        }
    }
}
