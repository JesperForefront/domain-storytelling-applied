namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookerTests
{
    [Fact]
    public void GivenAMunicipalityEmployee_WhenAccessedThroughBooker_ThenItExposesInheritedFields()
    {
        Booker booker = new MunicipalityEmployee(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"));

        AssertInheritedFields(booker);
    }

    [Fact]
    public void GivenAnAssociationRepresentative_WhenAccessedThroughBooker_ThenItExposesInheritedFields()
    {
        Booker booker = new AssociationRepresentative(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"),
            new NonBlankString("Computing Society"));

        AssertInheritedFields(booker);
    }

    [Fact]
    public void GivenAPrivateIndividual_WhenAccessedThroughBooker_ThenItExposesInheritedFields()
    {
        Booker booker = new PrivateIndividual(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"),
            new SocialSecurityNumber("123456-7890"));

        AssertInheritedFields(booker);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenABlankAssociation_WhenCreatingAnAssociationRepresentative_ThenItFailsExplicitly(string association)
    {
        Should.Throw<ArgumentException>(() => new AssociationRepresentative(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"),
            new NonBlankString(association)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenABlankSocialSecurityNumber_WhenCreatingAPrivateIndividual_ThenItFailsExplicitly(string socialSecurityNumber)
    {
        Should.Throw<ArgumentException>(() => new PrivateIndividual(
            new BookerId(Guid.NewGuid()),
            new NonBlankString("Ada"),
            new NonBlankString("Lovelace"),
            new EmailAddress("ada@example.test"),
            new SocialSecurityNumber(socialSecurityNumber)));
    }

    private static void AssertInheritedFields(Booker booker)
    {
        booker.Fname.Value.ShouldBe("Ada");
        booker.Lname.Value.ShouldBe("Lovelace");
        booker.Mail.Value.Value.ShouldBe("ada@example.test");
    }
}
