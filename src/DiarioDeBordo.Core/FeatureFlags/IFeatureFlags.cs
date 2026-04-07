namespace DiarioDeBordo.Core.FeatureFlags;

/// <summary>
/// Contrato de feature flags. Scaffolded in Phase 2 — implemented in Phase 3.
/// (Padrões Técnicos v4, seção 3.5)
/// </summary>
public interface IFeatureFlags
{
    bool IsEnabled(string flagName);
}

/// <summary>Placeholder: Phase 4 flags enabled. Full feature flag storage in Phase 3.</summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(Justification = "Phase 3 placeholder — feature flag storage not yet implemented.")]
public sealed class FeatureFlagsPlaceholder : IFeatureFlags
{
    private static readonly HashSet<string> _enabledFlags = new(StringComparer.OrdinalIgnoreCase)
    {
        "coletaneas",
        "fontes_com_fallback",
        "deduplicacao"
    };

    public bool IsEnabled(string flagName) => _enabledFlags.Contains(flagName);
}
