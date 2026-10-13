namespace TE.DomainStorytellingApplied.Application;

public class ReservationApplicationService
{
    private readonly IReservationOutcomeSender outcomeSender;

    public ReservationApplicationService(IReservationOutcomeSender outcomeSender)
    {
        ArgumentNullException.ThrowIfNull(outcomeSender);

        this.outcomeSender = outcomeSender;
    }

    public Reservation CreateReservation(
        ReservationId id,
        Booker booker,
        Timeslot timeslot,
        Premise premise,
        BookingPolicyDecision policyDecision)
    {
        var reservation = new Reservation(id, booker, timeslot, premise, policyDecision);

        if (reservation.Status == ReservationStatus.Approved)
        {
            SendOutcome(reservation);
        }

        return reservation;
    }

    public ReservationOutcome Approve(
        Reservation reservation,
        IReservationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        ArgumentNullException.ThrowIfNull(approver);

        reservation.Approve(approver);

        return SendOutcome(reservation);
    }

    public ReservationOutcome Reject(
        Reservation reservation,
        IReservationApprover approver)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        ArgumentNullException.ThrowIfNull(approver);

        reservation.Reject(approver);

        return SendOutcome(reservation);
    }

    private ReservationOutcome SendOutcome(Reservation reservation)
    {
        var outcome = ReservationOutcome.Create(reservation);
        outcomeSender.Send(outcome);

        return outcome;
    }
}
