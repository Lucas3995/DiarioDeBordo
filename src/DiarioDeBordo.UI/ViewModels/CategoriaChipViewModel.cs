using Avalonia.Media;

namespace DiarioDeBordo.UI.ViewModels;

/// <summary>Representa um chip de categoria no modal de detalhe (Plan 05).</summary>
public sealed class CategoriaChipViewModel
{
    public Guid Id { get; }
    public string Nome { get; }
    public bool IsAutomatica { get; }

    /// <summary>Nome exibido: automáticas recebem prefixo asterisco (D-11).</summary>
    public string NomeExibicao => IsAutomatica ? $"* {Nome}" : Nome;

    /// <summary>Fundo: automáticas recebem tint de acentuação (D-08/D-11).</summary>
    public IBrush Background =>
        IsAutomatica
            ? new SolidColorBrush(Color.FromArgb(38, 0, 120, 212)) // ~15% SystemAccentColor
            : Brushes.Transparent;

    public CategoriaChipViewModel(Guid id, string nome, bool isAutomatica)
    {
        Id = id;
        Nome = nome;
        IsAutomatica = isAutomatica;
    }
}
