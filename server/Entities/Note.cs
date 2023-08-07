namespace SecureNotes.Functions.Entities;

public class Note : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastUpdatedTime { get; set; } = DateTimeOffset.UtcNow;
}