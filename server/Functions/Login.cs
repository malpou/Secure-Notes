﻿using SecureNotes.Functions.Requests;
using SecureNotes.Functions.Responses;

namespace SecureNotes.Functions;

public partial class Functions
{
    [Function("Login")]
    public async Task<HttpResponseData> Login(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("Login");

        var loginRequest = await DeserializeRequestBodyAsync<UserRequest>(req);

        if (loginRequest == null || !IsValidUserRequest(loginRequest))
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

        var userResponse = new UserResponse
        {
            Token = token,
            Username = loginRequest.Username
        };

        logger.LogInformation("User logged in: {Username}", loginRequest.Username);

        return await CreateJsonResponseAsync(req, userResponse);
    }
}