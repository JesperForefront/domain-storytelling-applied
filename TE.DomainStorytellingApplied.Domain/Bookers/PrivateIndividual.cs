namespace TE.DomainStorytellingApplied.Domain;

public sealed class PrivateIndividual : Booker
{
    public PrivateIndividual(
        BookerId id,
        NonBlankString fname,
        NonBlankString lname,
        EmailAddress mail,
        SocialSecurityNumber ssn)
        : base(id, fname, lname, mail)
    {
        ArgumentNullException.ThrowIfNull(ssn);

        Ssn = ssn;
    }

    public SocialSecurityNumber Ssn { get; }
}
