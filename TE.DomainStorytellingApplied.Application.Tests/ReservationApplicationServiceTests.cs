namespace TE.DomainStorytellingApplied.Application.Tests;

public class ReservationApplicationServiceTests
{
    [Fact]
    public void GivenAutomaticApproval_WhenCreatingAReservation_ThenItSendsItsOutcome()
    {
        var sender = new RecordingOutcomeSender();
        var service = new ReservationApplicationService(sender);
        var request = CreateRequest();

        var reservation = service.CreateReservation(
            new ReservationId(Guid.NewGuid()),
            request.Booker,
            request.Timeslot,
            request.Premise,
            BookingPolicyDecision.AutomaticApproval);

        sender.Outcomes.ShouldHaveSingleItem();
        sender.Outcomes[0].ReservationId.ShouldBe(reservation.Id);
        sender.Outcomes[0].Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenReviewRequired_WhenCreatingAReservation_ThenItDoesNotSendAnOutcomeUntilItIsDecided()
    {
        var sender = new RecordingOutcomeSender();
        var service = new ReservationApplicationService(sender);
        var request = CreateRequest();

        var reservation = service.CreateReservation(
            new ReservationId(Guid.NewGuid()),
            request.Booker,
            request.Timeslot,
            request.Premise,
            BookingPolicyDecision.ReviewRequired);

        reservation.Status.ShouldBe(ReservationStatus.PendingApproval);
        sender.Outcomes.ShouldBeEmpty();
    }

    [Fact]
    public void GivenAPendingReservation_WhenApprovingIt_ThenItSendsAnApprovedOutcome()
    {
        var sender = new RecordingOutcomeSender();
        var service = new ReservationApplicationService(sender);
        var reservation = CreatePendingReservation(service);

        var outcome = service.Approve(reservation, new Approver(CreateBooker()));

        outcome.Status.ShouldBe(ReservationStatus.Approved);
        sender.Outcomes.ShouldHaveSingleItem().ShouldBe(outcome);
    }

    [Fact]
    public void GivenAPendingReservation_WhenRejectingIt_ThenItSendsARejectedOutcome()
    {
        var sender = new RecordingOutcomeSender();
        var service = new ReservationApplicationService(sender);
        var reservation = CreatePendingReservation(service);

        var outcome = service.Reject(reservation, new Approver(CreateBooker()));

        outcome.Status.ShouldBe(ReservationStatus.Rejected);
        sender.Outcomes.ShouldHaveSingleItem().ShouldBe(outcome);
    }

    private static Reservation CreatePendingReservation(ReservationApplicationService service)
    {
        var request = CreateRequest();

        return service.CreateReservation(
            new ReservationId(Guid.NewGuid()),
            request.Booker,
            request.Timeslot,
            request.Premise,
            BookingPolicyDecision.ReviewRequired);
    }

    private static (MunicipalityEmployee Booker, Timeslot Timeslot, Premise Premise) CreateRequest()
    {
        var premiseId = new PremiseId(Guid.NewGuid());

        return (
            CreateBooker(),
            new Timeslot(
                new TimeslotId(Guid.NewGuid()),
                new DateTimeOffset(2026, 10, 13, 9, 0, 0, TimeSpan.Zero),
                new Duration(3_600),
                premiseId),
            new Premise(
                premiseId,
                new NonBlankString("Town Hall"),
                new Address("1 Main Street")));
    }

    private static MunicipalityEmployee CreateBooker()
    {
        return new MunicipalityEmployee(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"));
    }

    private sealed class RecordingOutcomeSender : IReservationOutcomeSender
    {
        public List<ReservationOutcome> Outcomes { get; } = [];

        public void Send(ReservationOutcome outcome)
        {
            Outcomes.Add(outcome);
        }
    }
}
