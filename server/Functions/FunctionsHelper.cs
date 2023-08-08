using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;
using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions;

public partial class Functions
{
    private static async Task<T?> DeserializeRequestBodyAsync<T>(HttpRequestData req)
    {
        var request = await new StreamReader(req.Body).ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(request);
    }

    private static async Task<HttpResponseData> CreateJsonResponseAsync<T>(HttpRequestData req, T data,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");

        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        await response.WriteStringAsync(JsonConvert.SerializeObject(data, serializerSettings));

        return response;
    }

    private static bool IsValidUserRequest(UserRequest request)
    {
        return !string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password);
    }

    private static bool IsValidNoteRequest(NoteRequest request)
    {
        return !string.IsNullOrEmpty(request.Title) && !string.IsNullOrEmpty(request.Content);
    }

    private UserToken ExtractAndValidateToken(HttpRequestData req)
    {
        if (!JwtHelper.TryExtractTokenFromHeaders(req, out var token))
            return new UserToken();

        var principal = _jwtHelper.Validate(token);
        var username = JwtHelper.ExtractUsernameFromPrincipal(principal);

        return new UserToken {Principal = principal, Username = username};
    }

    private async Task<User?> FetchUserFromToken(UserToken token)
    {
        if (token.Username == null)
            return null;

        return await _userService.GetUser(token.Username);
    }

    private HttpResponseData GenerateErrorResponse(HttpRequestData req, HttpStatusCode code)
    {
        return req.CreateResponse(code);
    }

    private void LogInformation(ILogger logger, string action, string noteId)
    {
        logger.LogInformation("Note with ID {NoteId} {Action}", noteId, action);
    }

    public class UserToken
    {
        public ClaimsPrincipal? Principal { get; init; }
        public string? Username { get; init; }
    }
}