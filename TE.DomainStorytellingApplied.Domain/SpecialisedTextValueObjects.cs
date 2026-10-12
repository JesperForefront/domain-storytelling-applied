namespace TE.DomainStorytellingApplied.Domain;

public sealed record EmailAddress
{
    public EmailAddress(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}

public sealed record SocialSecurityNumber
{
    public SocialSecurityNumber(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}

public sealed record Address
{
    public Address(string value)
    {
        Value = new NonBlankString(value);
    }

    public NonBlankString Value { get; }
}
