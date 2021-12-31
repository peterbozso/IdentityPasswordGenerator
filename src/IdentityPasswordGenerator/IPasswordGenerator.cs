using Microsoft.AspNetCore.Identity;

namespace IdentityPasswordGenerator;

public interface IPasswordGenerator
{
    /// <summary>
    /// Generates a password according to the provided ASP.NET Core Identity password options.
    /// </summary>
    /// <param name="options">The ASP.NET Core Identity password options that the generated password must satisfy.</param>
    /// <param name="excludeNonRequiredChars">If set to true, characters that are not required by the options parameter will be excluded from the generated password. Defaults to false.</param>
    /// <returns>The generated password.</returns>
    string GeneratePassword(PasswordOptions options, bool excludeNonRequiredChars = false);
}
