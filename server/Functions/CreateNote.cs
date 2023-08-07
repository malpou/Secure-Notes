using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SecureNotes.Functions.Helpers;
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

        if (!JwtHelper.TryExtractTokenFromHeaders(req, out var token))
            return req.CreateResponse(HttpStatusCode.Unauthorized);

        var principal = _jwtHelper.Validate(token);

        var username = JwtHelper.ExtractUsernameFromPrincipal(principal);
        if (username == null) return req.CreateResponse(HttpStatusCode.Unauthorized);

        var noteRequest = await FunctionHelpers.DeserializeRequestBodyAsync<NoteRequest>(req);

        if (noteRequest == null || !FunctionHelpers.IsValidNoteRequest(noteRequest))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var user = await _userService.GetUser(username);
        if (user == null) return req.CreateResponse(HttpStatusCode.Unauthorized);


        var note = await _noteService.CreateAsync(
            noteRequest,
            user.RowKey);

        var noteResponse = new NoteResponse
        {
            Title = note.Title,
            Content = noteRequest.Content,
            CreatedAt = note.CreatedTime,
            UpdatedAt = note.LastUpdatedTime
        };

        logger.LogInformation("Note created: {Title} ({Id})", note.Title, note.RowKey);

        return await FunctionHelpers.CreateJsonResponseAsync(req, noteResponse);
    }
}