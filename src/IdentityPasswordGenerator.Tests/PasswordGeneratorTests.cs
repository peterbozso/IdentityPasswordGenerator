using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace IdentityPasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    private static readonly bool[] BoolValues = new[] { false, true };

    private readonly PasswordGenerator _passwordGenerator = new();

    [Fact]
    public void GeneratePassword_AllCharsExcluded_ThrowsException()
    {
        var options = new PasswordOptions
        {
            RequiredLength = 0,
            RequiredUniqueChars = 0,
            RequireNonAlphanumeric = false,
            RequireLowercase = false,
            RequireUppercase = false,
            RequireDigit = false
        };

        var exception = Assert.Throws<ArgumentException>(
            () => _passwordGenerator.GeneratePassword(options, true));

        Assert.Equal(
            "At least one set of characters must be required if excludeNonRequiredChars is set to true! (Parameter 'options')",
            exception.Message);
    }

    [Fact]
    public void GeneratePassword_RequiredUniqueCharsIsBiggerThanMaxUniqueChars_ThrowsException()
    {
        var maxUniqueChars =
            PasswordGenerator.NonAlphanumerics.Length + PasswordGenerator.Digits.Length;

        var options = new PasswordOptions
        {
            RequiredLength = 0,
            RequiredUniqueChars = maxUniqueChars + 1,
            RequireNonAlphanumeric = true,
            RequireLowercase = false,
            RequireUppercase = false,
            RequireDigit = true
        };

        var exception = Assert.Throws<ArgumentException>(
            () => _passwordGenerator.GeneratePassword(options, true));

        Assert.Equal(
            $"The value of RequiredUniqueChars cannot be bigger than the maximum number of unique characters ({maxUniqueChars})! (Parameter 'options')",
            exception.Message);
    }

    [Theory]
    [MemberData(nameof(ValidOptions))]
    public async Task GeneratePassword_ValidOptions_IsValid(
        PasswordOptions options,
        bool excludeNonRequiredChars,
        IEnumerable<string> excludedChars)
    {
        var password = _passwordGenerator.GeneratePassword(options, excludeNonRequiredChars);

        Assert.True(await ValidatePasswordAsync(options, password));

        if (excludeNonRequiredChars)
        {
            foreach (var chars in excludedChars)
            {
                Assert.True(password.All(c => !chars.Contains(c)));
            }
        }
    }

    public static IEnumerable<object[]> ValidOptions
    {
        get
        {
            var data = new List<object[]>();

            foreach (var requireNonAlphanumeric in BoolValues)
            {
                foreach (var requireLowercase in BoolValues)
                {
                    foreach (var requireUppercase in BoolValues)
                    {
                        foreach (var requireDigit in BoolValues)
                        {
                            var options = new PasswordOptions
                            {
                                RequiredLength = 20,
                                RequiredUniqueChars =
                                    (requireNonAlphanumeric ? PasswordGenerator.NonAlphanumerics.Length : 0)
                                    + (requireLowercase ? PasswordGenerator.Lowercases.Length : 0)
                                    + (requireUppercase ? PasswordGenerator.Uppercases.Length : 0)
                                    + (requireDigit ? PasswordGenerator.Digits.Length : 0),
                                RequireNonAlphanumeric = requireNonAlphanumeric,
                                RequireLowercase = requireLowercase,
                                RequireUppercase = requireUppercase,
                                RequireDigit = requireDigit
                            };

                            foreach (var excludeNonRequiredChars in BoolValues)
                            {
                                if (!excludeNonRequiredChars
                                    || requireNonAlphanumeric
                                    || requireLowercase
                                    || requireUppercase
                                    || requireDigit)
                                {
                                    var excludedChars = new List<string>();

                                    if (excludeNonRequiredChars)
                                    {
                                        if (!requireNonAlphanumeric)
                                        {
                                            excludedChars.Add(PasswordGenerator.NonAlphanumerics);
                                        }
                                        if (!requireLowercase)
                                        {
                                            excludedChars.Add(PasswordGenerator.Lowercases);
                                        }
                                        if (!requireUppercase)
                                        {
                                            excludedChars.Add(PasswordGenerator.Uppercases);
                                        }
                                        if (!requireDigit)
                                        {
                                            excludedChars.Add(PasswordGenerator.Digits);
                                        }
                                    }

                                    data.Add(new object[] {
                                        options,
                                        excludeNonRequiredChars,
                                        excludedChars
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }
    }

    private static async Task<bool> ValidatePasswordAsync(PasswordOptions passwordOptions, string password)
    {
        var validator = new PasswordValidator<IdentityUser>();

        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(um => um.Value).Returns(new IdentityOptions
        {
            Password = passwordOptions
        });

        var userManager = new UserManager<IdentityUser>(
            new Mock<IUserStore<IdentityUser>>().Object,
            options.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        var user = new IdentityUser("test");

        var result = await validator.ValidateAsync(userManager, user, password);

        return result.Succeeded;
    }
}
