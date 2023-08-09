namespace SecureNotes.Functions.Helpers;

public static class HashingHelper
{
    public static string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return hashedPassword;
    }

    public static bool ValidatePassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public static string HashData(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        var sb = new StringBuilder();
        foreach (var t in hashBytes) sb.Append(t.ToString("X2"));

        return sb.ToString();
    }

    public static bool ValidateHash(string input, string hash, out string newHash)
    {
        newHash = HashData(input);
        var comparer = StringComparer.OrdinalIgnoreCase;
        return 0 == comparer.Compare(newHash, hash);
    }
}