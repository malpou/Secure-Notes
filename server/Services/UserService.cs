using Azure.Security.KeyVault.Keys;
using SecureNotes.Functions.Entities;
using SecureNotes.Functions.Helpers;

namespace SecureNotes.Functions.Services;

public class UserService
{
    private readonly JwtHelper _jwtHelper;
    private readonly KeyHelper _keyHelper;
    private readonly TableStorageHelper<User> _tableStorageHelper;

    public UserService(TableStorageHelper<User> tableStorageHelper, JwtHelper jwtHelper, KeyHelper keyHelper)
    {
        _tableStorageHelper = tableStorageHelper;
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
            PasswordHash = passwordHash,
            CreatedTime = DateTimeOffset.UtcNow
        };

        var keyName = "master-key-" + user.RowKey;
        await _keyHelper.CreateKeyAsync(keyName, KeyType.RsaHsm);

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
        return await _tableStorageHelper.GetEntityByPartitionKeyAsync(username);
    }
}