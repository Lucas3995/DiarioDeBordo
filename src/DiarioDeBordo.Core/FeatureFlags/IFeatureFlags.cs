namespace DiarioDeBordo.Core.FeatureFlags;

/// <summary>
/// Contrato de feature flags. Scaffolded in Phase 2 — implemented in Phase 3.
/// (Padrões Técnicos v4, seção 3.5)
/// </summary>
public interface IFeatureFlags
{
    bool IsEnabled(string flagName);
}

/// <summary>Placeholder: all flags off by default until Phase 3 implements feature flag storage.</summary>
public sealed class FeatureFlagsPlaceholder : IFeatureFlags
{
    public bool IsEnabled(string flagName) => false;
}
