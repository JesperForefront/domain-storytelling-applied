namespace TE.DomainStorytellingApplied.Domain;

public sealed record SocialSecurityNumber
{
    public SocialSecurityNumber(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
