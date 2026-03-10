using DiarioDeBordo.Api.Composition;
using DiarioDeBordo.Api.Middleware;
using DiarioDeBordo.Application;
using DiarioDeBordo.Application.Status;
using DiarioDeBordo.Domain.Auth;
using DiarioDeBordo.Domain.Common;
using DiarioDeBordo.Infrastructure;
using DiarioDeBordo.Persistence;
using DiarioDeBordo.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "FrontendCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? ["http://localhost:4200"];

        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddApplication();
builder.Services.AddSingleton<IAmbiente, HostAmbienteAdapter>();
builder.Services.AddInfrastructure(builder.Configuration);

var skipPersistence = builder.Environment.IsEnvironment("Testing");
if (!skipPersistence)
    builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info.Title = "Diário de Bordo API";
        document.Info.Version = "v1";
        document.Info.Description = "API do sistema de acompanhamento de obras (manga, manhwa, anime, livro, filme, série).";
        return Task.CompletedTask;
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Configura JWT Bearer em runtime para que IConfiguration seja resolvido após build,
// permitindo que testes injetem configurações via WebApplicationFactory.
builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ConfigureNamedOptions<JwtBearerOptions>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Chave JWT não configurada. Defina Jwt__Key via variável de ambiente ou secrets.");
            var jwtIssuer = configuration["Jwt:Issuer"] ?? "diariodebordo";
            var jwtAudience = configuration["Jwt:Audience"] ?? "diariodebordo-clients";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
});

builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Seed de dados em Development (equivalente ao mock do frontend)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetService<DiarioDeBordoDbContext>();
    if (dbContext is not null)
    {
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        await ObrasSeed.AplicarAsync(dbContext, clock);

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await UsuariosSeed.AplicarAsync(dbContext, passwordHasher);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Title = "Diário de Bordo API");
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
