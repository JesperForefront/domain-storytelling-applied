namespace TE.DomainStorytellingApplied.Domain.Tests;

public class BookingPolicyDecisionTests
{
    [Fact]
    public void GivenBookingPolicyDecisions_WhenListingSupportedOutcomes_ThenItIncludesAutomaticApprovalAndReviewRequired()
    {
        var decisions = Enum.GetValues<BookingPolicyDecision>();

        decisions.Length.ShouldBe(2);
        decisions.ShouldContain(BookingPolicyDecision.AutomaticApproval);
        decisions.ShouldContain(BookingPolicyDecision.ReviewRequired);
    }
}
