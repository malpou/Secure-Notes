using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
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
        var userId = "";

        var tokenInfo = ExtractAndValidateToken(req);
        var user = await FetchUserFromToken(tokenInfo);
        if (user != null)
            userId = user.RowKey;

        var notes = await _noteService.GetAllAsync(userId, page ?? 1, pageSize ?? 5);
        var noteArray = notes as Note[] ?? notes.ToArray();

        var noteResponses = noteArray.Select(note => new NoteResponse
        {
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedTime,
            UpdatedAt = note.LastUpdatedTime
        }).ToList();

        logger.LogInformation("Notes retrieved: {Count}", noteArray.Length);
        return await CreateJsonResponseAsync(req, noteResponses);
    }
}