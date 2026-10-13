namespace TE.DomainStorytellingApplied.Domain.Tests;

public class TimeslotTests
{
    [Fact]
    public void GivenAValidTimeslot_WhenCreatingIt_ThenItIsAvailableByDefault()
    {
        var timeslot = CreateTimeslot();

        timeslot.IsAvailable.ShouldBeTrue();
    }

    [Fact]
    public void GivenATimeslot_WhenCreatingIt_ThenItRetainsItsPremiseIdentity()
    {
        var premiseId = new PremiseId(Guid.NewGuid());

        var timeslot = CreateTimeslot(premiseId);

        timeslot.PremiseId.ShouldBe(premiseId);
    }

    private static Timeslot CreateTimeslot(PremiseId? premiseId = null)
    {
        return new Timeslot(
            new TimeslotId(Guid.NewGuid()),
            new DateTimeOffset(2026, 10, 13, 9, 0, 0, TimeSpan.Zero),
            new Duration(3_600),
            premiseId ?? new PremiseId(Guid.NewGuid()));
    }
}
