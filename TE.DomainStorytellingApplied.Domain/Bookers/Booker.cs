namespace TE.DomainStorytellingApplied.Domain;

public abstract class Booker
{
    protected Booker(BookerId id, NonBlankString fname, NonBlankString lname, EmailAddress mail)
    {
        ArgumentNullException.ThrowIfNull(fname);
        ArgumentNullException.ThrowIfNull(lname);
        ArgumentNullException.ThrowIfNull(mail);

        Id = id;
        Fname = fname;
        Lname = lname;
        Mail = mail;
    }

    public BookerId Id { get; }

    public NonBlankString Fname { get; }

    public NonBlankString Lname { get; }

    public EmailAddress Mail { get; }
}
