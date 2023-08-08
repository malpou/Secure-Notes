using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SecureNotes.Functions.Requests;
using SecureNotes.Functions.Responses;

namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("UpdateNote")]
    public async Task<HttpResponseData> UpdateNote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "note/{noteId}")]
        HttpRequestData req,
        string noteId,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("UpdateNote");
        var tokenInfo = ExtractAndValidateToken(req);
        var user = await FetchUserFromToken(tokenInfo);

        if (user == null)
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);

        var noteRequest = await DeserializeRequestBodyAsync<NoteRequest>(req);

        if (noteRequest == null || !IsValidNoteRequest(noteRequest))
            return GenerateErrorResponse(req, HttpStatusCode.BadRequest);

        try
        {
            var updatedNote = await _noteService.UpdateAsync(user.RowKey, noteId, noteRequest);
            if (updatedNote == null)
            {
                logger.LogWarning("Note with ID {NoteId} not found", noteId);
                return GenerateErrorResponse(req, HttpStatusCode.NotFound);
            }

            var noteResponse = new NoteResponse
            {
                Title = updatedNote.Title,
                Content = noteRequest.Content,
                CreatedAt = updatedNote.CreatedTime,
                UpdatedAt = updatedNote.LastUpdatedTime
            };

            LogInformation(logger, "updated", noteId);
            return await CreateJsonResponseAsync(req, noteResponse);
        }
        catch (ArgumentException)
        {
            return GenerateErrorResponse(req, HttpStatusCode.BadRequest);
        }
    }
}