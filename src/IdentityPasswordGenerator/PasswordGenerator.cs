using Microsoft.AspNetCore.Identity;

namespace IdentityPasswordGenerator;

public class PasswordGenerator : IPasswordGenerator
{
    /// <summary>
    /// All non-alphanumeric characters.
    /// </summary>
    public const string NonAlphanumerics = "!@#$%^&*";

    /// <summary>
    /// All lower case ASCII characters.
    /// </summary>
    public const string Lowercases = "abcdefghijkmnopqrstuvwxyz";

    /// <summary>
    /// All upper case ASCII characters.
    /// </summary>
    public const string Uppercases = "ABCDEFGHJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// All digits.
    /// </summary>
    public const string Digits = "0123456789";

    private static readonly Requirement[] Requirements =
        new Requirement[]
        {
            new(NonAlphanumerics, nameof(PasswordOptions.RequireNonAlphanumeric)),
            new(Lowercases, nameof(PasswordOptions.RequireLowercase)),
            new(Uppercases, nameof(PasswordOptions.RequireUppercase)),
            new(Digits, nameof(PasswordOptions.RequireDigit))
        };

    private readonly OptionsValidator _optionsValidator = new(Requirements);
    private readonly CryptoRandom _random = new();

    public string GeneratePassword(PasswordOptions options, bool excludeNonRequiredChars = false)
    {
        _optionsValidator.ThrowIfInvalid(options, excludeNonRequiredChars);

        var password = new Password(_random);

        SatisfyCharacterTypeRequirements(password, options);
        SatisfyLengthRequirements(password, options, excludeNonRequiredChars);

        return password.ToString();
    }

    private static void SatisfyCharacterTypeRequirements(Password password, PasswordOptions options)
    {
        foreach (var requirement in Requirements)
        {
            if (requirement.IsRequired(options))
            {
                password.InsertRandom(requirement.Chars);
            }
        }
    }

    private void SatisfyLengthRequirements(
        Password password,
        PasswordOptions options,
        bool excludeNonRequiredChars)
    {
        Requirement GetRandomRequirement() =>
            Requirements[_random.Next(0, Requirements.Length)];

        for (var i = password.Length;
            i < options.RequiredLength || password.UniqueChars < options.RequiredUniqueChars;
            i++)
        {
            var requirement = GetRandomRequirement();

            if (excludeNonRequiredChars)
            {
                while (!requirement.IsRequired(options))
                {
                    requirement = GetRandomRequirement();
                }
            }

            password.InsertRandom(requirement.Chars);
        }
    }
}
