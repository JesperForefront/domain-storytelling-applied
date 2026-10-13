namespace TE.DomainStorytellingApplied.Domain;

public record Address
{
    public Address(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
