using System.Security.Cryptography;
using DiarioDeBordo.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace DiarioDeBordo.Tests.Unit;

public class DataProtectionServiceTests
{
    private static IConfiguration CriarConfigComChave(string? keyBase64 = null)
    {
        keyBase64 ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DataProtection:Key"] = keyBase64,
            })
            .Build();
        return config;
    }

    [Fact]
    public void Construtor_SemChaveConfigurada_LancaExcecao()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        Assert.Throws<InvalidOperationException>(() => new DataProtectionService(config));
    }

    [Fact]
    public void Construtor_ChaveComTamanhoErrado_LancaExcecao()
    {
        var chaveCurta = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        Assert.Throws<InvalidOperationException>(() => new DataProtectionService(CriarConfigComChave(chaveCurta)));
    }

    [Fact]
    public void Criptografar_Descriptografar_RetornaValorOriginal()
    {
        var sut = new DataProtectionService(CriarConfigComChave());

        var resultado = sut.Criptografar("texto sensível");
        var decifrado = sut.Descriptografar(resultado);

        Assert.Equal("texto sensível", decifrado);
    }

    [Fact]
    public void Criptografar_GeraOutputDiferenteCadaVez()
    {
        var sut = new DataProtectionService(CriarConfigComChave());

        var r1 = sut.Criptografar("mesmo texto");
        var r2 = sut.Criptografar("mesmo texto");

        Assert.NotEqual(r1, r2); // nonces diferentes
    }

    [Fact]
    public void Descriptografar_DadosInvalidos_LancaCryptographicException()
    {
        var sut = new DataProtectionService(CriarConfigComChave());

        var dadosCurtos = Convert.ToBase64String(new byte[10]);

        Assert.Throws<CryptographicException>(() => sut.Descriptografar(dadosCurtos));
    }

    [Fact]
    public void Descriptografar_DadosAdulterados_LancaExcecao()
    {
        var sut = new DataProtectionService(CriarConfigComChave());

        var cifrado = sut.Criptografar("original");
        var bytes = Convert.FromBase64String(cifrado);
        bytes[^1] ^= 0xFF; // adultera o tag
        var adulterado = Convert.ToBase64String(bytes);

        Assert.ThrowsAny<CryptographicException>(() => sut.Descriptografar(adulterado));
    }

    [Fact]
    public void Criptografar_StringVazia_FuncionaCorretamente()
    {
        var sut = new DataProtectionService(CriarConfigComChave());

        var cifrado = sut.Criptografar("");
        var decifrado = sut.Descriptografar(cifrado);

        Assert.Equal("", decifrado);
    }

    [Fact]
    public void Criptografar_TextoLongo_FuncionaCorretamente()
    {
        var sut = new DataProtectionService(CriarConfigComChave());
        var textoLongo = new string('A', 10_000);

        var cifrado = sut.Criptografar(textoLongo);
        var decifrado = sut.Descriptografar(cifrado);

        Assert.Equal(textoLongo, decifrado);
    }
}
