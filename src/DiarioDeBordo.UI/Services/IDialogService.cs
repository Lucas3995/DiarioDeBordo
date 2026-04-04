namespace DiarioDeBordo.UI.Services;

/// <summary>
/// Abstração de serviços de diálogo para MVVM — permite mock em testes e remove acoplamento de Window do ViewModel.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Abre o modal de detalhe de um conteúdo.
    /// Retorna true se o conteúdo foi modificado (necessita refresh da lista),
    /// false se cancelado sem mudanças, null se excluído.
    /// </summary>
    Task<bool?> MostrarConteudoDetalheAsync(Guid conteudoId);

    /// <summary>
    /// Exibe um diálogo de confirmação genérico (D-02, D-03).
    /// Retorna true se o usuário clicou no botão primário, false se cancelou.
    /// </summary>
    Task<bool> MostrarConfirmacaoAsync(
        string titulo,
        string mensagem,
        string botaoPrimario,
        string botaoSecundario,
        bool isPrimarioDestructivo = false);
}
