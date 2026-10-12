namespace TE.DomainStorytellingApplied.Domain.Tests;

public class SpecialisedTextValueObjectTests
{
    [Fact]
    public void GivenANonBlankEmailAddress_WhenCreatingAnEmailAddress_ThenItRetainsTheValue()
    {
        var emailAddress = new EmailAddress("not-an-email");

        emailAddress.Value.Value.ShouldBe("not-an-email");
    }

    [Fact]
    public void GivenANonBlankSocialSecurityNumber_WhenCreatingASocialSecurityNumber_ThenItRetainsTheValue()
    {
        var socialSecurityNumber = new SocialSecurityNumber("not-a-social-security-number");

        socialSecurityNumber.Value.Value.ShouldBe("not-a-social-security-number");
    }

    [Fact]
    public void GivenANonBlankAddress_WhenCreatingAnAddress_ThenItRetainsTheValue()
    {
        var address = new Address("1 Main Street");

        address.Value.Value.ShouldBe("1 Main Street");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenBlankText_WhenCreatingASpecialisedTextValueObject_ThenItFailsExplicitly(string value)
    {
        Should.Throw<ArgumentException>(() => new EmailAddress(value));
        Should.Throw<ArgumentException>(() => new SocialSecurityNumber(value));
        Should.Throw<ArgumentException>(() => new Address(value));
    }

    [Fact]
    public void GivenNullText_WhenCreatingASpecialisedTextValueObject_ThenItFailsExplicitly()
    {
        Should.Throw<ArgumentNullException>(() => new EmailAddress(null!));
        Should.Throw<ArgumentNullException>(() => new SocialSecurityNumber(null!));
        Should.Throw<ArgumentNullException>(() => new Address(null!));
    }
}
