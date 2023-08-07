namespace SecureNotes.Functions.Entities;

public class User : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
    public DateTimeOffset LastLoginTime { get; set; }
}