namespace IdentityPasswordGenerator;

internal class Password
{
    private readonly List<char> _text = new();
    private readonly CryptoRandom _random;

    public Password(CryptoRandom random)
    {
        _random = random;
    }

    public int Length => _text.Count;

    public int UniqueChars => _text.Distinct().Count();

    public void InsertRandom(string chars) =>
        _text.Insert(_random.Next(0, _text.Count), chars[_random.Next(0, chars.Length)]);

    public override string ToString() => new(_text.ToArray());
}
