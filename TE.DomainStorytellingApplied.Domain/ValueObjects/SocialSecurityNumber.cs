namespace TE.DomainStorytellingApplied.Domain;

public record SocialSecurityNumber
{
    public SocialSecurityNumber(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
