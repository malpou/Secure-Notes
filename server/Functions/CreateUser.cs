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
    [Function("CreateUser")]
    public async Task<HttpResponseData> CreateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "register")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("CreateUser");

        var loginRequest = await FunctionHelpers.DeserializeRequestBodyAsync<UserRequest>(req);

        if (loginRequest == null || !FunctionHelpers.IsValidUserRequest(loginRequest))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        string token;
        try
        {
            token = await _userService.CreateUser(loginRequest.Username, loginRequest.Password);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating user: {Username}", loginRequest.Username);
            return req.CreateResponse(HttpStatusCode.Conflict);
        }

        var userResponse = new UserResponse {Token = token};

        logger.LogInformation("User created: {Username}", loginRequest.Username);

        return await FunctionHelpers.CreateJsonResponseAsync(req, userResponse);
    }
}