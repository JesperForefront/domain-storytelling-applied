namespace TE.DomainStorytellingApplied.Domain.Tests;

public class PremiseTests
{
    [Fact]
    public void GivenANameAndAddress_WhenCreatingAPremise_ThenItRetainsThem()
    {
        var name = new NonBlankString("Town Hall");
        var address = new Address("1 Main Street");

        var premise = new Premise(new PremiseId(Guid.NewGuid()), name, address);

        premise.Name.ShouldBe(name);
        premise.Address.ShouldBe(address);
    }
}
