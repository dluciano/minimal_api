using AutoFixture;

namespace MinimalApi.Tests;

internal static class AutoDomainDataHelper
{
    public static ICustomization[] ToCustomization(this Type[] customizationTypes)
    {
        var customizations = customizationTypes
            .Select(customizationType =>
            {
                return Activator.CreateInstance(customizationType) is ICustomization customization ?
                    customization :
                    throw new InvalidCastException("Customization cannot be created");
            }).ToArray();

        return customizations;
    }
}
