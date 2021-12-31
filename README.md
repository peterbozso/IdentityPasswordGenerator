# IdentityPasswordGenerator

IdentityPasswordGenerator is a library for generating passwords based on ASP.NET Core Identity password options.

## Usage

Get the package from [NuGet.](https://www.nuget.org/packages/IdentityPasswordGenerator/)

Then in your code:

```csharp
using IdentityPasswordGenerator;
using Microsoft.AspNetCore.Identity;

var passwordGenerator = new PasswordGenerator();
var options = new PasswordOptions();
var password = passwordGenerator.GeneratePassword(options);
```

See [IPasswordGenerator](src/IdentityPasswordGenerator/IPasswordGenerator.cs) for more details.

## Credits

The idea of this library is coming from [this Stack Overflow answer.](https://stackoverflow.com/a/46229180/3078296)
