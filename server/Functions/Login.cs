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
    [Function("Login")]
    public async Task<HttpResponseData> Login(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("Login");
        logger.LogInformation("Login process started");

        var loginRequest = await FunctionHelpers.DeserializeRequestBodyAsync<UserRequest>(req);

        if (loginRequest == null || !FunctionHelpers.IsValidUserRequest(loginRequest))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        string token;
        try
        {
            token = await _userService.Login(loginRequest.Username, loginRequest.Password);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error logging in user: {Username}", loginRequest.Username);
            return req.CreateResponse(HttpStatusCode.Unauthorized);
        }

        logger.LogInformation("User logged in: {Username}", loginRequest.Username);
        var userResponse = new UserResponse {Token = token};

        logger.LogInformation("Login process finished");

        return await FunctionHelpers.CreateJsonResponseAsync(req, userResponse);
    }
}