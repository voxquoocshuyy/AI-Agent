using AutoFixture;
using AutoFixture.Xunit2;

namespace AI.Agent.UnitTests.TestHelpers;

public static class TestHelper
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        // fixture.Customize(new AutoMoqCustomization());
        return fixture;
    }
}

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() => TestHelper.CreateFixture())
    {
    }
}

public class InlineAutoMoqDataAttribute : InlineAutoDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(new AutoMoqDataAttribute(), values)
    {
    }
}