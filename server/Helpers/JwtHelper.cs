namespace SecureNotes.Functions.Helpers;

public class JwtHelper
{
    private readonly string _secret;

    public JwtHelper(string secret)
    {
        _secret = secret;
    }

    public string Generate(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("username", username)
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal Validate(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    public static bool TryExtractTokenFromHeaders(HttpRequestData req, out string extractedToken)
    {
        extractedToken = string.Empty;

        if (!req.Headers.Contains("Authorization"))
            return false;

        extractedToken = req.Headers.GetValues("Authorization").First().Replace("Bearer ", "");
        return true;
    }

    public static string? ExtractUsernameFromPrincipal(ClaimsPrincipal principal)
    {
        return principal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
    }
}