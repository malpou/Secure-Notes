namespace SecureNotes.Functions.Helpers;

public class KeyHelper
{
    private readonly KeyClient _keyClient;

    public KeyHelper(string keyVaultUrl)
    {
        _keyClient = new KeyClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }

    public async Task<KeyVaultKey> CreateKeyAsync(string keyName, KeyType keyType)
    {
        var key = await _keyClient.CreateKeyAsync(keyName, keyType);
        return key.Value;
    }

    public async Task<byte[]> EncryptWithAzureKeyVault(byte[] data, string keyName)
    {
        var key = await GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Id, new DefaultAzureCredential());
        var result = await cryptoClient.EncryptAsync(EncryptionAlgorithm.RsaOaep256, data);
        return result.Ciphertext;
    }

    public async Task<byte[]> DecryptWithAzureKeyVault(byte[] encryptedData, string keyName)
    {
        var key = await GetKeyAsync(keyName);
        var cryptoClient = new CryptographyClient(key.Id, new DefaultAzureCredential());
        var result = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep256, encryptedData);
        return result.Plaintext;
    }

    private async Task<KeyVaultKey> GetKeyAsync(string keyName)
    {
        var key = await _keyClient.GetKeyAsync("master-key-" + keyName);
        return key.Value;
    }

    public async Task DeleteKeyAsync(string keyName)
    {
        var deleteOperation = await _keyClient.StartDeleteKeyAsync(keyName);

        await deleteOperation.WaitForCompletionAsync();

        await _keyClient.PurgeDeletedKeyAsync(keyName);
    }
}