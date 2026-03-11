using DiarioDeBordo.Application.Auth.Login;
using DiarioDeBordo.Application.Common;
using DiarioDeBordo.Application.Echo;
using DiarioDeBordo.Application.Obras.AtualizarPosicao;
using DiarioDeBordo.Application.Obras.Listar;
using DiarioDeBordo.Application.Obras.ObterPorIdOuNome;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using DiarioDeBordo.Application.Auth;

namespace DiarioDeBordo.Application;

/// <summary>
/// Registra todos os serviços da camada Application no contêiner de DI.
/// Chamado pela camada Api (Frameworks/Drivers) durante a composição raiz.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // autenticação extraída: reduz responsabilidade do handler
        services.AddTransient<IAuthenticationService, AuthenticationService>();

        services.AddTransient<IRequestValidator<EchoCommand>, FluentValidationRequestValidatorAdapter<EchoCommand>>();
        services.AddTransient<IRequestValidator<LoginCommand>, FluentValidationRequestValidatorAdapter<LoginCommand>>();
        services.AddTransient<IRequestValidator<GetObrasAcompanhamentoQuery>, FluentValidationRequestValidatorAdapter<GetObrasAcompanhamentoQuery>>();
        services.AddTransient<IRequestValidator<AtualizarPosicaoObraCommand>, FluentValidationRequestValidatorAdapter<AtualizarPosicaoObraCommand>>();
        services.AddTransient<IRequestValidator<GetObraPorIdOuNomeQuery>, FluentValidationRequestValidatorAdapter<GetObraPorIdOuNomeQuery>>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
