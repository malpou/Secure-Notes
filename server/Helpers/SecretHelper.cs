namespace SecureNotes.Functions.Helpers;

public class SecretHelper
{
    private readonly SecretClient _secretClient;

    public SecretHelper(string keyVaultUrl)
    {
        _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var secret = await _secretClient.GetSecretAsync(secretName);
        return secret.Value.Value;
    }

    public async Task SetSecretAsync(string secretName, string secretValue)
    {
        await _secretClient.SetSecretAsync(secretName, secretValue);
    }
}