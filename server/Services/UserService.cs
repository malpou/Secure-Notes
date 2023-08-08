using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;

namespace SecureNotes.Functions.Services;

public class UserService
{
    private readonly JwtHelper _jwtHelper;
    private readonly KeyHelper _keyHelper;
    private readonly NoteService _noteService;
    private readonly TableStorageHelper<User> _tableStorageHelper;

    public UserService(TableStorageHelper<User> tableStorageHelper, NoteService noteService, JwtHelper jwtHelper,
        KeyHelper keyHelper)
    {
        _tableStorageHelper = tableStorageHelper;
        _noteService = noteService;
        _jwtHelper = jwtHelper;
        _keyHelper = keyHelper;
    }

    public async Task<string> CreateUser(string username, string password)
    {
        var passwordHash = HashingHelper.HashPassword(password);

        if (!await _tableStorageHelper.IsValueInColumnUniqueAsync("PartitionKey", username))
            throw new Exception("User already exists");

        var user = new User
        {
            PartitionKey = username,
            RowKey = Guid.NewGuid().ToString(),
            PasswordHash = passwordHash
        };

        var keyName = "master-key-" + user.RowKey;
        await _keyHelper.CreateKeyAsync(keyName, KeyType.Rsa);

        await _tableStorageHelper.AddEntityAsync(user);

        return await Login(username, password);
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await GetUser(username);

        if (user == null)
            throw new Exception("User does not exist");

        if (!HashingHelper.ValidatePassword(password, user.PasswordHash))
            throw new Exception("Invalid password");

        user.LastLoginTime = DateTimeOffset.UtcNow;

        await _tableStorageHelper.UpdateEntityAsync(user, user.ETag);

        return _jwtHelper.Generate(user.PartitionKey);
    }

    public async Task<User?> GetUser(string username)
    {
        return await _tableStorageHelper.GetEntityByColumnAsync("PartitionKey", username);
    }

    public async Task<bool> DeleteUser(string username, ILogger log)
    {
        var user = await GetUser(username);
        if (user == null) return false;

        // Remove all associated notes
        if (!await _noteService.DeleteAllNotes(user))
            log.LogError("Failed to delete all notes for user {Username} ({UserId})", username, user.RowKey);

        // Remove the associated key
        var keyName = "master-key-" + user.RowKey;
        await _keyHelper.DeleteKeyAsync(keyName);

        // Delete the user
        await _tableStorageHelper.DeleteEntityAsync(user.PartitionKey, user.RowKey);

        return true;
    }


    public Task<User?> GetUserByRowKey(string rowKey)
    {
        return _tableStorageHelper.GetEntityByColumnAsync("RowKey", rowKey);
    }
}