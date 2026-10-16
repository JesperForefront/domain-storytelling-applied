using System.Runtime.CompilerServices;



/*
    Jag ser var du tänker med detta Value Object, och det finns en rimlighet i att bara ha en för alla strings som inte får vara tomma!
    Den biten är att man inte behöver duplicera den logiken på fler ställen.

    Men det finns en till vinning i Value Objects och det är att ge semantisk mening till ett Value Object.
    Detta för att visa på att dom REPRESENTERAR olika saker, även om dom fungerar på samma sätt under huven.

    Jag kommer slänga en hel del ändringar i denna filen med lite olika nivåer på hur man kan göra detta :)
*/



// Steg 1 - Olika objekt med olika namn för att dom representerar olika objekt. Men "duplicerad" validering.
namespace Step1
{
    public sealed record Name
    {
        public string Value { get; private init; }

        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{value}' is not a valid Name");

            Value = value;
        }
    }

    public sealed record Description
    {
        public string Value { get; private init; }

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{value}' is not a valid Name");

            Value = value;
        }
    }

    // Nackdelen är att man behöver skriva valideringen överallt
}


// Steg 2 - En gemensam Guard clause
namespace Step2
{
    public sealed class Guards
    {
        public static void EnsureIsNotEmpty(string value, [CallerMemberName] string? caller = "value")
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{value}' is not a valid ${caller}"); // caller här kommer vara Name eller Description beroende på vem som anropar.
        }
    }

    public sealed record Name
    {
        public string Value { get; private set; }

        public Name(string value)
        {
            Guards.EnsureIsNotEmpty(value);

            Value = value;
        }
    }

    public sealed record Description
    {
        public string Value { get; private set; }

        public Description(string value)
        {
            Guards.EnsureIsNotEmpty(value);

            Value = value;
        }
    }
}



// Step 3 - Base record/class.
namespace Step3
{
    public abstract record StringValueObject
    {
        public string Value { get; private set; }

        protected StringValueObject(string value)
        {
            // Här kan man absolut använda Guards.IsNotEmpty också, men kan kanske bli lite knivigare att få tag i rätt namn så att det blir Name/Description.
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{value}' is not a valid {GetType().Name}"); // GetType().Name blir inte StringValueObject utan Name eller Description.

            Value = value;
        }
    }

    public sealed record Name : StringValueObject
    {
        public Name(string value) : base(value) { }
    }

    public sealed record Description : StringValueObject
    {
        public Description(string value) : base(value) { }
    }
}

// Dom är alla nyanser av det och man får hitta vad man trivs bäst med. Finns nog flera sätt att göra det på men min preferens är vanligen #3.



namespace TE.DomainStorytellingApplied.Domain
{
    public record NonBlankString
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
}