namespace TE.DomainStorytellingApplied.Domain;

public class Reservation
{
    public Reservation(
        ReservationId id,
        Booker booker, // Booker borde nog vara ett ID här också då den ligger i ett eget aggregat
        Timeslot timeslot, // Samma sak här skulle jag tro då Timeslots antingen är ett eget aggregat eller bor under Premises
        Premise premise, // Premises är ett eget aggregat skulle jag säga då den lever sitt eget liv och inte styrs direkt av Reservation. Men Reservation beror på det!
        BookingPolicyDecision policyDecision)
    {
        ArgumentNullException.ThrowIfNull(booker);
        ArgumentNullException.ThrowIfNull(timeslot);
        ArgumentNullException.ThrowIfNull(premise);

        if (timeslot.PremiseId != premise.Id)
        {
            throw new ArgumentException("Timeslot must belong to the requested premise.", nameof(premise));
        }

        // Om vi skulle referera Timeslot bara via ID så skulle vi inte kunna göra denna checken här.
        // Då är det aktuellt med en Domain Service som skulle sköta orkestereringen av denna invariant!
        // Anledningen till att man vill använda ID istället för en referens är på grund av något som kallas Consistency Boundary.
        // Du kan läsa mer om det här:
        // https://www.arhohuttunen.com/domain-driven-design-repositories-application-services/
        // https://milanjovanovic.tech/blog/application-services-vs-domain-services-ddd
        //
        // Det var något vi inte hann dyka in i så djupt under passet tyvärr.
        // Mycket handlar om hur man designar så man kan läsa och skriva saker utan att behöva spara hela sin domän model varje gång. Utan kunna jobba med ett Aggregat åt gången.
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

    // Snyggt!
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
