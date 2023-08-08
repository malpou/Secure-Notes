namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("DeleteNote")]
    public async Task<HttpResponseData> DeleteNote(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "note/{noteId}")]
        HttpRequestData req, string noteId, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("DeleteNote");
        var token = ExtractAndValidateToken(req);

        if (token.Principal == null)
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);

        var user = await FetchUserFromToken(token);

        if (user == null)
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);

        var isDeleted = await _noteService.DeleteAsync(user.RowKey, noteId);
        if (!isDeleted)
        {
            LogInformation(logger, "not found", noteId);
            return GenerateErrorResponse(req, HttpStatusCode.NotFound);
        }

        LogInformation(logger, "deleted", noteId);
        return GenerateErrorResponse(req, HttpStatusCode.OK);
    }
}