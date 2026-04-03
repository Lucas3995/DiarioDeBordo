using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DiarioDeBordo.UI.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly Func<AcervoViewModel> _acervoVmFactory;

    [ObservableProperty]
    private object? _activeViewModel;

    public IReadOnlyList<NavigationItem> NavigationItems { get; }

    public MainViewModel(Func<AcervoViewModel> acervoVmFactory)
    {
        ArgumentNullException.ThrowIfNull(acervoVmFactory);
        _acervoVmFactory = acervoVmFactory;

        NavigationItems =
        [
            new NavigationItem("Acervo", () => NavigateTo("acervo")),
            new NavigationItem("Agregação", () => NavigateTo("agregacao")),
            new NavigationItem("Busca", () => NavigateTo("busca")),
            new NavigationItem("Configurações", () => NavigateTo("config")),
        ];

        // Start at Acervo (the only active section in Phase 2)
        NavigateTo("acervo");
    }

    private void NavigateTo(string section)
    {
        ActiveViewModel = section switch
        {
            "acervo" => _acervoVmFactory(),
            "agregacao" => new PlaceholderViewModel("Agregação"),
            "busca" => new PlaceholderViewModel("Busca"),
            "config" => new PlaceholderViewModel("Configurações"),
            _ => new PlaceholderViewModel(section),
        };
    }
}

public sealed class NavigationItem
{
    public string Titulo { get; }
    public IRelayCommand NavegarCommand { get; }

    public NavigationItem(string titulo, Action navegar)
    {
        ArgumentNullException.ThrowIfNull(titulo);
        ArgumentNullException.ThrowIfNull(navegar);
        Titulo = titulo;
        NavegarCommand = new RelayCommand(navegar);
    }
}

public sealed class PlaceholderViewModel
{
    public string SectionName { get; }
    public string Message { get; }

    public PlaceholderViewModel(string sectionName)
    {
        ArgumentNullException.ThrowIfNull(sectionName);
        SectionName = sectionName;
        Message = $"Seção '{sectionName}' estará disponível em versão futura.";
    }
}
