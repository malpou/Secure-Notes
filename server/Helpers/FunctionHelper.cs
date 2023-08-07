using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SecureNotes.Functions.Requests;

namespace SecureNotes.Functions.Helpers;

public static class FunctionHelpers
{
    public static async Task<T?> DeserializeRequestBodyAsync<T>(HttpRequestData req)
    {
        var request = await new StreamReader(req.Body).ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(request);
    }

    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(HttpRequestData req, T data,
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

    public static bool IsValidUserRequest(UserRequest request)
    {
        return !string.IsNullOrEmpty(request.Username) && !string.IsNullOrEmpty(request.Password);
    }

    public static bool IsValidNoteRequest(NoteRequest request)
    {
        return !string.IsNullOrEmpty(request.Title) && !string.IsNullOrEmpty(request.Content);
    }
}