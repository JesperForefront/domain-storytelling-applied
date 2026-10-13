namespace TE.DomainStorytellingApplied.Domain;

public readonly record struct Duration
{
    public Duration(int seconds)
    {
        if (seconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Duration must be positive.");
        }

        Seconds = seconds;
    }

    public int Seconds { get; }
}
