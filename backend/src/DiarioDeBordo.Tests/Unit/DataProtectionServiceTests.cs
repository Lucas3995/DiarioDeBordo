using System.Security.Cryptography;
using DiarioDeBordo.Infrastructure.Security;

namespace DiarioDeBordo.Tests.Unit;

public class DataProtectionServiceTests
{
    private static Microsoft.Extensions.Options.IOptions<DataProtectionOptions> CriarOptionsComChave(string? keyBase64 = null)
    {
        keyBase64 ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var opts = new DataProtectionOptions(keyBase64);
        return Microsoft.Extensions.Options.Options.Create(opts);
    }

    [Fact]
    public void Construtor_SemChaveConfigurada_LancaExcecao()
    {
        // opções explícitas com chave nula
        var options = Microsoft.Extensions.Options.Options.Create(new DataProtectionOptions(null!));
        Assert.Throws<InvalidOperationException>(() => new DataProtectionService(options));
    }

    [Fact]
    public void Construtor_ChaveComTamanhoErrado_LancaExcecao()
    {
        var chaveCurta = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        var options = CriarOptionsComChave(chaveCurta);
        Assert.Throws<InvalidOperationException>(() => new DataProtectionService(options));
    }

    [Fact]
    public void Criptografar_Descriptografar_RetornaValorOriginal()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());

        var resultado = sut.Criptografar("texto sensível");
        var decifrado = sut.Descriptografar(resultado);

        Assert.Equal("texto sensível", decifrado);
    }

    [Fact]
    public void Criptografar_GeraOutputDiferenteCadaVez()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());

        var r1 = sut.Criptografar("mesmo texto");
        var r2 = sut.Criptografar("mesmo texto");

        Assert.NotEqual(r1, r2); // nonces diferentes
    }

    [Fact]
    public void Descriptografar_DadosInvalidos_LancaCryptographicException()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());

        var dadosCurtos = Convert.ToBase64String(new byte[10]);

        Assert.Throws<CryptographicException>(() => sut.Descriptografar(dadosCurtos));
    }

    [Fact]
    public void Descriptografar_DadosAdulterados_LancaExcecao()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());

        var cifrado = sut.Criptografar("original");
        var bytes = Convert.FromBase64String(cifrado);
        bytes[^1] ^= 0xFF; // adultera o tag
        var adulterado = Convert.ToBase64String(bytes);

        Assert.ThrowsAny<CryptographicException>(() => sut.Descriptografar(adulterado));
    }

    [Fact]
    public void Criptografar_StringVazia_FuncionaCorretamente()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());

        var cifrado = sut.Criptografar("");
        var decifrado = sut.Descriptografar(cifrado);

        Assert.Equal("", decifrado);
    }

    [Fact]
    public void Criptografar_TextoLongo_FuncionaCorretamente()
    {
        var sut = new DataProtectionService(CriarOptionsComChave());
        var textoLongo = new string('A', 10_000);

        var cifrado = sut.Criptografar(textoLongo);
        var decifrado = sut.Descriptografar(cifrado);

        Assert.Equal(textoLongo, decifrado);
    }
}
