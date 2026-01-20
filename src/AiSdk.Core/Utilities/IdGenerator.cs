using System.Security.Cryptography;
using System.Text;

namespace AiSdk.Core.Utilities;

/// <summary>
/// Generates unique IDs similar to nanoid.
/// </summary>
public static class IdGenerator
{
    private const string DefaultAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Generates a unique ID with the specified prefix and size.
    /// </summary>
    /// <param name="prefix">The prefix for the ID (e.g., "aitxt").</param>
    /// <param name="size">The size of the random portion (default: 24).</param>
    /// <returns>A unique ID string.</returns>
    public static string Generate(string prefix = "aitxt", int size = 24)
    {
        ArgumentNullException.ThrowIfNull(prefix);

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than 0.");
        }

        var random = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        var result = new StringBuilder(prefix.Length + size + 1);
        result.Append(prefix);
        result.Append('-');

        foreach (var b in random)
        {
            result.Append(DefaultAlphabet[b % DefaultAlphabet.Length]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Generates a simple unique ID without prefix.
    /// </summary>
    /// <param name="size">The size of the ID (default: 24).</param>
    /// <returns>A unique ID string.</returns>
    public static string GenerateSimple(int size = 24)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be greater than 0.");
        }

        var random = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(random);
        }

        var result = new StringBuilder(size);
        foreach (var b in random)
        {
            result.Append(DefaultAlphabet[b % DefaultAlphabet.Length]);
        }

        return result.ToString();
    }
}
