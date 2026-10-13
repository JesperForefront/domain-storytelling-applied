namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ReservationTests
{
    [Fact]
    public void GivenATimeslotForADifferentPremise_WhenCreatingAReservation_ThenItFailsExplicitly()
    {
        var timeslot = CreateTimeslot();
        var differentPremise = CreatePremise();

        Should.Throw<ArgumentException>(() => CreateReservation(
            BookingPolicyDecision.AutomaticApproval,
            timeslot,
            differentPremise));
    }

    [Fact]
    public void GivenAnUnavailableTimeslot_WhenCreatingAReservation_ThenItFailsExplicitly()
    {
        var timeslot = CreateTimeslot(isAvailable: false);
        var premise = CreatePremise(timeslot.PremiseId);

        Should.Throw<InvalidOperationException>(() => CreateReservation(
            BookingPolicyDecision.AutomaticApproval,
            timeslot,
            premise));
    }

    [Fact]
    public void GivenAutomaticApproval_WhenCreatingAReservation_ThenItIsApproved()
    {
        var reservation = CreateReservation(BookingPolicyDecision.AutomaticApproval);

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenReviewRequired_WhenCreatingAReservation_ThenItIsPendingApproval()
    {
        var reservation = CreateReservation(BookingPolicyDecision.ReviewRequired);

        reservation.Status.ShouldBe(ReservationStatus.PendingApproval);
    }

    [Fact]
    public void GivenAPendingReservation_WhenAnApproverApprovesIt_ThenItIsApproved()
    {
        var reservation = CreateReservation(BookingPolicyDecision.ReviewRequired);

        reservation.Approve(CreateApprover());

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    [Fact]
    public void GivenAPendingReservation_WhenAnApproverRejectsIt_ThenItIsRejected()
    {
        var reservation = CreateReservation(BookingPolicyDecision.ReviewRequired);

        reservation.Reject(CreateApprover());

        reservation.Status.ShouldBe(ReservationStatus.Rejected);
    }

    [Fact]
    public void GivenADecidedReservation_WhenAnotherDecisionIsAttempted_ThenItFailsAndPreservesTheOriginalOutcome()
    {
        var reservation = CreateReservation(BookingPolicyDecision.AutomaticApproval);

        Should.Throw<InvalidOperationException>(() => reservation.Reject(CreateApprover()));

        reservation.Status.ShouldBe(ReservationStatus.Approved);
    }

    private static Reservation CreateReservation(
        BookingPolicyDecision policyDecision,
        Timeslot? timeslot = null,
        Premise? premise = null)
    {
        var resolvedTimeslot = timeslot ?? CreateTimeslot();
        var resolvedPremise = premise ?? CreatePremise(resolvedTimeslot.PremiseId);

        return new Reservation(
            new ReservationId(Guid.NewGuid()),
            CreateBooker(),
            resolvedTimeslot,
            resolvedPremise,
            policyDecision);
    }

    private static MunicipalityEmployee CreateBooker()
    {
        return new MunicipalityEmployee(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"));
    }

    private static Approver CreateApprover()
    {
        return new Approver(CreateBooker());
    }

    private static Premise CreatePremise(PremiseId? id = null)
    {
        return new Premise(
            id ?? new PremiseId(Guid.NewGuid()),
            new NonBlankString("Town Hall"),
            new Address("1 Main Street"));
    }

    private static Timeslot CreateTimeslot(bool isAvailable = true)
    {
        return new Timeslot(
            new TimeslotId(Guid.NewGuid()),
            new DateTimeOffset(2026, 10, 13, 9, 0, 0, TimeSpan.Zero),
            new Duration(3_600),
            new PremiseId(Guid.NewGuid()),
            isAvailable);
    }
}
