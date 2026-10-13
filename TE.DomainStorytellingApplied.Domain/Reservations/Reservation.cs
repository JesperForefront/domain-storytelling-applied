namespace TE.DomainStorytellingApplied.Domain;

public class Reservation
{
    public Reservation(
        ReservationId id,
        Booker booker,
        Timeslot timeslot,
        Premise premise,
        BookingPolicyDecision policyDecision)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(timeslot);
        ArgumentNullException.ThrowIfNull(premise);

        if (timeslot.PremiseId != premise.Id)
        {
            throw new ArgumentException("Timeslot must belong to the requested premise.", nameof(premise));
        }

        if (!timeslot.IsAvailable)
        {
            throw new InvalidOperationException("An unavailable timeslot cannot be reserved.");
        }

        Id = id;
        BookerId = booker.Id;
        TimeslotId = timeslot.Id;
        PremiseId = premise.Id;
        PolicyDecision = policyDecision;
        Status = policyDecision switch
        {
            BookingPolicyDecision.AutomaticApproval => ReservationStatus.Approved,
            BookingPolicyDecision.ReviewRequired => ReservationStatus.PendingApproval,
            _ => throw new ArgumentOutOfRangeException(
                nameof(policyDecision),
                policyDecision,
                "Booking policy decision is not supported.")
        };
    }

    public ReservationId Id { get; }

    public BookerId BookerId { get; }

    public TimeslotId TimeslotId { get; }

    public PremiseId PremiseId { get; }

    public BookingPolicyDecision PolicyDecision { get; }

    public ReservationStatus Status { get; private set; }

    public void Approve(IReservationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(approver);
        EnsurePendingApproval();

        Status = ReservationStatus.Approved;
    }

    public void Reject(IReservationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(approver);
        EnsurePendingApproval();

        Status = ReservationStatus.Rejected;
    }

    private void EnsurePendingApproval()
    {
        if (Status != ReservationStatus.PendingApproval)
        {
            throw new InvalidOperationException("Only pending reservations can be decided.");
        }
    }
}
