namespace TE.DomainStorytellingApplied.Domain;

public record ReservationOutcome
{
    private ReservationOutcome(
        ReservationId reservationId,
        BookerId bookerId,
        ReservationStatus status)
    {
        ReservationId = reservationId;
        BookerId = bookerId;
        Status = status;
    }

    public ReservationId ReservationId { get; }

    public BookerId BookerId { get; }

    public ReservationStatus Status { get; }

    public static ReservationOutcome Create(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        if (reservation.Status == ReservationStatus.PendingApproval)
        {
            throw new InvalidOperationException("A pending reservation does not have an outcome.");
        }

        return new ReservationOutcome(
            reservation.Id,
            reservation.BookerId,
            reservation.Status);
    }
}
