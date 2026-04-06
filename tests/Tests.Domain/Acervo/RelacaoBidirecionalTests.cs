using DiarioDeBordo.Core.Entidades;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Testes da estrutura de Relacao e do padrão bidirecional two-row (D-15).
/// Não testa lógica de handler (auto-referência, duplicatas) — isso é Plan 03.
/// </summary>
public class RelacaoBidirecionalTests
{
    private static readonly Guid _usuarioId = Guid.NewGuid();
    private static readonly Guid _tipoRelacaoId = Guid.NewGuid();

    private static (Relacao forward, Relacao inverse) CriarPar(
        Guid origemId, Guid destinoId, Guid? tipoId = null)
    {
        var tipo = tipoId ?? _tipoRelacaoId;
        var forwardId = Guid.NewGuid();
        var inverseId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var forward = new Relacao
        {
            Id = forwardId,
            ConteudoOrigemId = origemId,
            ConteudoDestinoId = destinoId,
            TipoRelacaoId = tipo,
            IsInversa = false,
            ParId = inverseId,
            UsuarioId = _usuarioId,
            CriadoEm = now,
        };

        var inverse = new Relacao
        {
            Id = inverseId,
            ConteudoOrigemId = destinoId,
            ConteudoDestinoId = origemId,
            TipoRelacaoId = tipo,
            IsInversa = true,
            ParId = forwardId,
            UsuarioId = _usuarioId,
            CriadoEm = now,
        };

        return (forward, inverse);
    }

    [Fact]
    public void Given_ParCriado_When_ForwardRow_Then_IsInversaEhFalse()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, _) = CriarPar(origemId, destinoId);

        Assert.False(forward.IsInversa);
    }

    [Fact]
    public void Given_ParCriado_When_InverseRow_Then_IsInversaEhTrue()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (_, inverse) = CriarPar(origemId, destinoId);

        Assert.True(inverse.IsInversa);
    }

    [Fact]
    public void Given_ParCriado_When_ForwardParId_Then_ApontaParaInverseId()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, inverse) = CriarPar(origemId, destinoId);

        Assert.Equal(inverse.Id, forward.ParId);
    }

    [Fact]
    public void Given_ParCriado_When_InverseParId_Then_ApontaParaForwardId()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, inverse) = CriarPar(origemId, destinoId);

        Assert.Equal(forward.Id, inverse.ParId);
    }

    [Fact]
    public void Given_ParCriado_When_ForwardRow_Then_OrigemEDestinoCorretos()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, _) = CriarPar(origemId, destinoId);

        Assert.Equal(origemId, forward.ConteudoOrigemId);
        Assert.Equal(destinoId, forward.ConteudoDestinoId);
    }

    [Fact]
    public void Given_ParCriado_When_InverseRow_Then_OrigemEDestinoInvertidos()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (_, inverse) = CriarPar(origemId, destinoId);

        Assert.Equal(destinoId, inverse.ConteudoOrigemId);
        Assert.Equal(origemId, inverse.ConteudoDestinoId);
    }

    [Fact]
    public void Given_ParCriado_When_TipoRelacaoId_Then_AmbosUsaMesmoTipo()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, inverse) = CriarPar(origemId, destinoId);

        Assert.Equal(forward.TipoRelacaoId, inverse.TipoRelacaoId);
    }

    [Fact]
    public void Given_ParCriado_When_UsuarioId_Then_AmbosPertencenteAoMesmoUsuario()
    {
        var origemId = Guid.NewGuid();
        var destinoId = Guid.NewGuid();
        var (forward, inverse) = CriarPar(origemId, destinoId);

        Assert.Equal(_usuarioId, forward.UsuarioId);
        Assert.Equal(_usuarioId, inverse.UsuarioId);
    }
}
