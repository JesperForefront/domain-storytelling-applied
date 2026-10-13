namespace TE.DomainStorytellingApplied.Domain;

public record EmailAddress
{
    public EmailAddress(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
