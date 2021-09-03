using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;

namespace MinimalApi.Tests;

public sealed class AutoDomainDataAttribute : AutoDataAttribute
{
    public AutoDomainDataAttribute()
        : base(() => new Fixture().Customize(new CustomCustomization()))
    { }

    public AutoDomainDataAttribute(params Type[] customizations)
        : base(() => new Fixture().Customize(new CustomCustomization(customizations)))
    { }

    private sealed class CustomCustomization : CompositeCustomization
    {
        public CustomCustomization() : base(new[] { new AutoNSubstituteCustomization() })
        {
        }

        public CustomCustomization(Type[] customizations) : base(
            new[] { new AutoNSubstituteCustomization() }.Concat(customizations.ToCustomization())
            )
        {
        }
    }
}
