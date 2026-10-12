namespace TE.DomainStorytellingApplied.Domain;

public sealed record Address
{
    public Address(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
