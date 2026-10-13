namespace TE.DomainStorytellingApplied.Domain;

public sealed class Premise
{
    public Premise(PremiseId id, NonBlankString name, Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);

        Id = id;
        Name = name;
        Address = address;
    }

    public PremiseId Id { get; }

    public NonBlankString Name { get; }

    public Address Address { get; }
}
