using Microsoft.AspNetCore.Identity;

namespace IdentityPasswordGenerator;

internal class OptionsValidator
{
    private readonly Requirement[] _requirements;

    public OptionsValidator(Requirement[] requirements)
    {
        _requirements = requirements;
    }

    public void ThrowIfInvalid(PasswordOptions options, bool excludeNonRequiredChars)
    {
        var isAllCharsExcluded =
            excludeNonRequiredChars
            && _requirements.All(requirement => !requirement.IsRequired(options));

        if (isAllCharsExcluded)
        {
            throw new ArgumentException(
                $"At least one set of characters must be required if {nameof(excludeNonRequiredChars)} is set to true!",
                nameof(options));
        }

        var maxUniqueChars =
            _requirements
                .Where(r => r.IsRequired(options))
                .Select(r => r.Chars.Length)
                .Sum();

        if (excludeNonRequiredChars && options.RequiredUniqueChars > maxUniqueChars)
        {
            throw new ArgumentException(
                $"The value of {nameof(options.RequiredUniqueChars)} cannot be bigger than the maximum number of unique characters ({maxUniqueChars})!",
                nameof(options));
        }
    }
}
