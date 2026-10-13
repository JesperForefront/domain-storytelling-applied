namespace TE.DomainStorytellingApplied.Domain.Tests;

public class DurationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GivenAZeroOrNegativeNumberOfSeconds_WhenCreatingADuration_ThenItFailsExplicitly(int seconds)
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new Duration(seconds));
    }
}
