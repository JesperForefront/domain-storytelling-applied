namespace TE.DomainStorytellingApplied.Domain;

public class MunicipalityEmployee : Booker
{
    public MunicipalityEmployee(BookerId id, NonBlankString fname, NonBlankString lname, EmailAddress mail)
        : base(id, fname, lname, mail)
    {
    }
}
