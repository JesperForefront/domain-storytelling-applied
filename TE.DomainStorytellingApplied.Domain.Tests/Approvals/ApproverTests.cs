namespace TE.DomainStorytellingApplied.Domain.Tests;

public class ApproverTests
{
    [Fact]
    public void GivenAMunicipalityEmployee_WhenCreatingAnApprover_ThenItExposesThatEmployee()
    {
        var employee = CreateMunicipalityEmployee();

        var approver = new Approver(employee);

        approver.MunicipalityEmployee.ShouldBe(employee);
    }

    [Fact]
    public void GivenAnApprover_WhenAnApproverCapabilityIsRequired_ThenItCanBeSupplied()
    {
        var approver = new Approver(CreateMunicipalityEmployee());

        var suppliedApprover = RequireApprover(approver);

        suppliedApprover.ShouldBe(approver);
    }

    [Fact]
    public void GivenAMunicipalityEmployeeWithoutAnApproverRole_WhenAnApproverCapabilityIsRequired_ThenItCannotBeSupplied()
    {
        var employee = CreateMunicipalityEmployee();

        (employee is IReservationApprover).ShouldBeFalse();
    }

    private static MunicipalityEmployee CreateMunicipalityEmployee()
    {
        return new MunicipalityEmployee(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"));
    }

    private static IReservationApprover RequireApprover(IReservationApprover approver)
    {
        return approver;
    }
}
