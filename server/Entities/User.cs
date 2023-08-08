namespace SecureNotes.Functions.Entities;

public class User : BaseEntity
{
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastLoginTime { private get; set; } = DateTimeOffset.UtcNow;
}