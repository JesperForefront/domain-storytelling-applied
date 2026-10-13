namespace TE.DomainStorytellingApplied.Domain;

public class Approver : IReservationApprover
{
    public Approver(MunicipalityEmployee municipalityEmployee)
    {
        ArgumentNullException.ThrowIfNull(municipalityEmployee);

        MunicipalityEmployee = municipalityEmployee;
    }

    public MunicipalityEmployee MunicipalityEmployee { get; }
}
