using SecureNotes.Functions.Requests;
using SecureNotes.Functions.Responses;

namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("CreateNote")]
    public async Task<HttpResponseData> CreateNote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "note")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("CreateNote");
        var tokenInfo = ExtractAndValidateToken(req);
        var user = await FetchUserFromToken(tokenInfo);

        if (user == null)
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);

        var noteRequest = await DeserializeRequestBodyAsync<NoteRequest>(req);

        if (noteRequest == null || !IsValidNoteRequest(noteRequest))
            return GenerateErrorResponse(req, HttpStatusCode.BadRequest);

        var note = await _noteService.CreateAsync(noteRequest, user.RowKey);
        var noteResponse = new NoteResponse
        {
            Id = note.RowKey,
            Title = noteRequest.Title,
            Content = noteRequest.Content,
            CreatedAt = note.CreatedTime,
            UpdatedAt = note.LastUpdatedTime,
            Author = note.Author
        };

        LogInformation(logger, "created", note.RowKey);
        return await CreateJsonResponseAsync(req, noteResponse);
    }
}