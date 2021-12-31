using Microsoft.AspNetCore.Identity;

namespace IdentityPasswordGenerator;

internal class Requirement
{
    public Requirement(string chars, string propName)
    {
        Chars = chars;
        IsRequired = (options) =>
            (bool)typeof(PasswordOptions).GetProperty(propName)!.GetValue(options)!;
    }

    public string Chars { get; private set; }

    public Func<PasswordOptions, bool> IsRequired { get; private set; }
}
