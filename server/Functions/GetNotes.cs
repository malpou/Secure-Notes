using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Responses;

namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("GetNotes")]
    public async Task<HttpResponseData> GetNotes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "note")]
        HttpRequestData req,
        int? page,
        int? pageSize,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("GetAllNotes");
        
        var tokenInfo = ExtractAndValidateToken(req);
        var user = await FetchUserFromToken(tokenInfo);
        if (user == null)
        {
            logger.LogWarning("User not found");
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);
        }

        var notes = await _noteService.GetAllAsync(user, page ?? 1, pageSize ?? 5);
        var noteArray = notes as Note[] ?? notes.ToArray();

        var noteResponses = noteArray.Select(note => new NoteResponse
        {
            Id = note.RowKey,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedTime,
            UpdatedAt = note.LastUpdatedTime,
            Author = note.Author
        }).ToList();

        logger.LogInformation("Notes retrieved: {Count}", noteArray.Length);
        return await CreateJsonResponseAsync(req, noteResponses);
    }
}