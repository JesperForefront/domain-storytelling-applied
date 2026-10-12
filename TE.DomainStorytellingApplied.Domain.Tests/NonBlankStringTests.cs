namespace TE.DomainStorytellingApplied.Domain.Tests;

public class NonBlankStringTests
{
    [Fact]
    public void GivenANonBlankValue_WhenCreatingANonBlankString_ThenItRetainsTheValue()
    {
        var value = new NonBlankString("Main Street");

        value.Value.ShouldBe("Main Street");
    }

    [Fact]
    public void GivenANullValue_WhenCreatingANonBlankString_ThenItFailsExplicitly()
    {
        Should.Throw<ArgumentNullException>(() => new NonBlankString(null!));
    }

    [Fact]
    public void GivenAnEmptyValue_WhenCreatingANonBlankString_ThenItFailsExplicitly()
    {
        Should.Throw<ArgumentException>(() => new NonBlankString(string.Empty));
    }

    [Fact]
    public void GivenAWhitespaceOnlyValue_WhenCreatingANonBlankString_ThenItFailsExplicitly()
    {
        Should.Throw<ArgumentException>(() => new NonBlankString("   "));
    }
}
