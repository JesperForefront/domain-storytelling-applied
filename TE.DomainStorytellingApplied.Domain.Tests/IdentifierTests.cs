namespace TE.DomainStorytellingApplied.Domain.Tests;

public class IdentifierTests
{
    [Fact]
    public void GivenAGuid_WhenCreatingABookerId_ThenItRetainsTheGuid()
    {
        var guid = Guid.NewGuid();

        var identifier = new BookerId(guid);

        identifier.Value.ShouldBe(guid);
    }

    [Fact]
    public void GivenAGuid_WhenCreatingAPremiseId_ThenItRetainsTheGuid()
    {
        var guid = Guid.NewGuid();

        var identifier = new PremiseId(guid);

        identifier.Value.ShouldBe(guid);
    }

    [Fact]
    public void GivenAGuid_WhenCreatingATimeslotId_ThenItRetainsTheGuid()
    {
        var guid = Guid.NewGuid();

        var identifier = new TimeslotId(guid);

        identifier.Value.ShouldBe(guid);
    }

    [Fact]
    public void GivenAGuid_WhenCreatingAReservationId_ThenItRetainsTheGuid()
    {
        var guid = Guid.NewGuid();

        var identifier = new ReservationId(guid);

        identifier.Value.ShouldBe(guid);
    }
}
