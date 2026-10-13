namespace TE.DomainStorytellingApplied.Application;

public interface IReservationOutcomeSender
{
    void Send(ReservationOutcome outcome);
}
