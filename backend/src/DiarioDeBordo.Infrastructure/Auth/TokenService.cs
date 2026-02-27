using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiarioDeBordo.Application.Auth;
using DiarioDeBordo.Domain.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DiarioDeBordo.Infrastructure.Auth;

/// <summary>
/// Geração de tokens JWT assinados com HMAC-SHA256.
/// Lê chave, issuer e audience de IConfiguration (Jwt:Key, Jwt:Issuer, Jwt:Audience).
/// Expiração padrão: 8 horas.
/// </summary>
public sealed class TokenService : ITokenService
{
    private const int ExpiracaoHoras = 8;

    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Key' não encontrada.");
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Issuer' não encontrada.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Configuração 'Jwt:Audience' não encontrada.");
    }

    public (string Token, DateTime ExpiresAt) GerarToken(Usuario usuario)
    {
        var expiresAt = DateTime.UtcNow.AddHours(ExpiracaoHoras);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("login", usuario.Login),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
        };

        var keyBytes = Encoding.UTF8.GetBytes(_key);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return (tokenString, expiresAt);
    }
}
