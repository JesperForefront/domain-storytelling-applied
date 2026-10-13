namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationOutcomeTests
{
    [Fact]
    public void GivenAnApprovedReservation_WhenCreatingAnOutcome_ThenItContainsTheReservationBookerAndApprovedStatus()
    {
        var reservation = CreateReservation(BookingPolicyDecision.AutomaticApproval);

        var outcome = ReservationOutcome.Create(reservation);

        outcome.ReservationId.ShouldBe(reservation.Id);
        outcome.BookerId.ShouldBe(reservation.BookerId);
        outcome.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenARejectedReservation_WhenCreatingAnOutcome_ThenItContainsTheRejectedStatus()
    {
        var reservation = CreateReservation(BookingPolicyDecision.ReviewRequired);
        reservation.Reject(CreateApprover());

        var outcome = ReservationOutcome.Create(reservation);

        outcome.Status.ShouldBe(ReservationStatus.Rejected);
    }

    [Fact]
    public void GivenAPendingReservation_WhenCreatingAnOutcome_ThenItFailsExplicitly()
    {
        var reservation = CreateReservation(BookingPolicyDecision.ReviewRequired);

        Should.Throw<InvalidOperationException>(() => ReservationOutcome.Create(reservation));
    }

    private static Reservation CreateReservation(BookingPolicyDecision policyDecision)
    {
        var premiseId = new PremiseId(Guid.NewGuid());

        return new Reservation(
            new ReservationId(Guid.NewGuid()),
            CreateBooker(),
            new Timeslot(
                new TimeslotId(Guid.NewGuid()),
                new DateTimeOffset(2026, 10, 13, 9, 0, 0, TimeSpan.Zero),
                new Duration(3_600),
                premiseId),
            new Premise(
                premiseId,
                new NonBlankString("Town Hall"),
                new Address("1 Main Street")),
            policyDecision);
    }

    private static Approver CreateApprover()
    {
        return new Approver(CreateBooker());
    }

    private static MunicipalityEmployee CreateBooker()
    {
        return new MunicipalityEmployee(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"));
    }
}
