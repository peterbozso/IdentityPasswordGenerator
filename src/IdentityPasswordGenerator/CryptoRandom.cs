using System.Security.Cryptography;

namespace IdentityPasswordGenerator;

internal class CryptoRandom
{
    public int Next(int fromInclusive, int toExclusive)
    {
        if (fromInclusive == toExclusive)
        {
            return fromInclusive;
        }

        return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
    }
}
