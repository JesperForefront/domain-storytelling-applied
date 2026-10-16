namespace TE.DomainStorytellingApplied.Domain;

public class Timeslot
{
    public Timeslot(
        TimeslotId id,
        DateTimeOffset start,
        Duration duration,
        PremiseId premiseId,
        bool isAvailable = true)
    {
        // Nu när du har Duration som Value Object så kan vi skippa denna check :)
        if (duration.Seconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Duration must be positive.");
        }

        Id = id;
        Start = start;
        Duration = duration;
        PremiseId = premiseId;
        IsAvailable = isAvailable;
    }

    public TimeslotId Id { get; }

    public DateTimeOffset Start { get; }

    public Duration Duration { get; }

    public PremiseId PremiseId { get; }

    public bool IsAvailable { get; }
}
