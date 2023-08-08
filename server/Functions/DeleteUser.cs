namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("DeleteUser")]
    public async Task<HttpResponseData> DeleteUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "account")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("DeleteUser");
        var tokenInfo = ExtractAndValidateToken(req);
        var user = await FetchUserFromToken(tokenInfo);

        if (user == null)
            return GenerateErrorResponse(req, HttpStatusCode.Unauthorized);

        bool isDeleted;
        try
        {
            isDeleted = await _userService.DeleteUser(user.PartitionKey, logger);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting user: {Username}", user.PartitionKey);
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }

        if (!isDeleted)
            return req.CreateResponse(HttpStatusCode.NotFound);

        logger.LogInformation("User deleted: {Username}", user.PartitionKey);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}