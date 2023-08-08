using System.Security.Cryptography;
using System.Text;

namespace SecureNotes.Functions.Helpers;

public class CryptographyHelper
{
    private readonly KeyHelper _keyHelper;

    public CryptographyHelper(KeyHelper keyHelper)
    {
        _keyHelper = keyHelper;
    }

    public async Task<string> EncryptData(byte[] data, string userId)
    {
        var dek = GenerateRandomKey();

        var encryptedData = EncryptWithAes(data, dek);

        var encryptedDek = await _keyHelper.EncryptWithAzureKeyVault(dek, userId);

        return Convert.ToBase64String(encryptedDek) + ":" + Convert.ToBase64String(encryptedData);
    }

    public async Task<string> DecryptData(string encryptedBlob, string userId)
    {
        var parts = encryptedBlob.Split(':');
        var encryptedDek = Convert.FromBase64String(parts[0]);
        var encryptedData = Convert.FromBase64String(parts[1]);

        var dek = await _keyHelper.DecryptWithAzureKeyVault(encryptedDek, userId);

        var decryptedBytes = DecryptWithAes(encryptedData, dek);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    private static byte[] EncryptWithAes(byte[] data, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);

        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        var cipherText = memStream.ToArray();
        var result = new byte[aes.IV.Length + cipherText.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);

        return result;
    }

    private static byte[] DecryptWithAes(byte[] encryptedData, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var iv = new byte[16];
        Array.Copy(encryptedData, 0, iv, 0, iv.Length);
        var actualCipherText = new byte[encryptedData.Length - iv.Length];
        Array.Copy(encryptedData, iv.Length, actualCipherText, 0, actualCipherText.Length);

        using var decrypt = aes.CreateDecryptor(key, iv);
        using var memStream = new MemoryStream(actualCipherText);
        using var cryptoStream = new CryptoStream(memStream, decrypt, CryptoStreamMode.Read);

        var plainText = new byte[actualCipherText.Length];
        var bytesRead = cryptoStream.Read(plainText, 0, plainText.Length);

        var actualPlainText = new byte[bytesRead];
        Array.Copy(plainText, 0, actualPlainText, 0, bytesRead);

        return actualPlainText;
    }

    private static byte[] GenerateRandomKey()
    {
        var randomKey = new byte[32];
        RandomNumberGenerator.Fill(randomKey);
        return randomKey;
    }
}