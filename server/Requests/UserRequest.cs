namespace SecureNotes.Functions.Requests;

public abstract class UserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}