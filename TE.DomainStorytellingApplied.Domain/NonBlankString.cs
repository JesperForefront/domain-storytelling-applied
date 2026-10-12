namespace TE.DomainStorytellingApplied.Domain;

public sealed record NonBlankString
{
    public NonBlankString(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value must not be empty or whitespace.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }
}
