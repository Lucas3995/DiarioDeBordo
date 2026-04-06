namespace DiarioDeBordo.UI.ViewModels;

/// <summary>ViewModel para o diálogo de confirmação genérico (D-02, D-03).</summary>
public sealed class ConfirmacaoDialogViewModel
{
    public string Titulo { get; }
    public string Mensagem { get; }
    public string BotaoPrimario { get; }
    public string BotaoSecundario { get; }
    public bool IsPrimarioDestructivo { get; }

    public ConfirmacaoDialogViewModel(
        string titulo,
        string mensagem,
        string botaoPrimario,
        string botaoSecundario,
        bool isPrimarioDestructivo = false)
    {
        Titulo = titulo;
        Mensagem = mensagem;
        BotaoPrimario = botaoPrimario;
        BotaoSecundario = botaoSecundario;
        IsPrimarioDestructivo = isPrimarioDestructivo;
    }
}
