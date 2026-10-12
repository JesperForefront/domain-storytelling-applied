namespace TE.DomainStorytellingApplied.Domain;

public sealed class AssociationRepresentative : Booker
{
    public AssociationRepresentative(
        BookerId id,
        NonBlankString fname,
        NonBlankString lname,
        EmailAddress mail,
        NonBlankString association)
        : base(id, fname, lname, mail)
    {
        ArgumentNullException.ThrowIfNull(association);

        Association = association;
    }

    public NonBlankString Association { get; }
}
