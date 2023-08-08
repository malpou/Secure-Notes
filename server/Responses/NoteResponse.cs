using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Responses;

public class NoteResponse : NoteRequest
{
    public DateTimeOffset CreatedAt { private get; set; }
    public DateTimeOffset UpdatedAt { private get; set; }
}