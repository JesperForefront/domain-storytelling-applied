namespace TE.DomainStorytellingApplied.Domain;

public class AssociationRepresentative : Booker
{
    public AssociationRepresentative(
        BookerId id,
        // Jag skulle vilja slå ett slag för att man kostar på sig längre variabel namn.
        // Känner man att saker blir otroligt långa i namngivningen så har man nog valt fel abstraktion och något gör för mycket. 
        // Då blir det tillbaka till vår vän SOLID
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
