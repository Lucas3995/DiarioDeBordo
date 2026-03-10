using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DiarioDeBordo.Tests.Integration;

/// <summary>
/// WebApplicationFactory configurada para testes de integração.
/// Define ambiente "Testing" (sem persistência) e injeta secrets mínimos via in-memory.
/// </summary>
public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "chave-de-teste-com-32-caracteres-ok!" },
                { "Jwt:Issuer", "diariodebordo-test" },
                { "Jwt:Audience", "diariodebordo-clients-test" },
                { "DataProtection:Key", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=" }
            });
        });
    }
}
