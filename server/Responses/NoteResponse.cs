using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Responses;

public class NoteResponse : NoteRequest
{
    public string Id { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string Author { get; set; } = string.Empty;
}