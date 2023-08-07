using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Responses;

public class NoteResponse : NoteRequest
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}