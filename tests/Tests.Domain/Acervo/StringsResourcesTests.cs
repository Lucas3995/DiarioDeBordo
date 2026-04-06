using System;
using System.Linq;
using System.Reflection;
using DiarioDeBordo.Module.Acervo.Resources;
using Xunit;

namespace DiarioDeBordo.Tests.Domain.Acervo;

/// <summary>
/// Validates that every string property in <see cref="Strings"/> can be loaded at runtime.
/// A MissingManifestResourceException here means the .resx is not embedded correctly —
/// fix the .csproj EmbeddedResource / RootNamespace configuration.
/// </summary>
public class StringsResourcesTests
{
    private static readonly PropertyInfo[] AllStringProperties =
        typeof(Strings)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();

    [Fact]
    public void TodosOsStrings_DeveCarregarSemExcecao()
    {
        // Uses reflection so new strings are automatically included without
        // changing this test — a MissingManifestResourceException will fail here.
        Assert.NotEmpty(AllStringProperties);

        foreach (var prop in AllStringProperties)
        {
            var ex = Record.Exception(() => prop.GetValue(null));
            Assert.Null(ex);
        }
    }

    [Fact]
    public void TodosOsStrings_NaoDevemRetornarNuloOuVazio()
    {
        foreach (var prop in AllStringProperties)
        {
            var value = (string?)prop.GetValue(null);
            Assert.False(
                string.IsNullOrWhiteSpace(value),
                $"Strings.{prop.Name} returned null/empty — add missing entry to Strings.resx");
        }
    }
}
