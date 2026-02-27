using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DiarioDeBordo.Tests.Integration;

/// <summary>
/// Testes de integração mínimos que sobem a API in-memory via WebApplicationFactory.
/// Valida que a "fiação" da aplicação está correta (DI, middleware, roteamento).
/// 
/// O ambiente "Testing" é usado para ignorar a persistência (sem banco real necessário).
/// </summary>
public sealed class HealthCheckIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public HealthCheckIntegrationTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GET_health_DeveRetornar200()
    {
        // Act
        var resposta = await _client.GetAsync("/health");

        // Assert
        resposta.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_status_DeveRetornar200ComJson()
    {
        // Act
        var resposta = await _client.GetAsync("/status");

        // Assert
        resposta.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        resposta.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}
