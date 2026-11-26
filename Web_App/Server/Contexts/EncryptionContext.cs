using System.Security.Cryptography;
using System.Text;

namespace Server.Contexts;

/// <summary>
/// Hashing is a one-way process that converts a password to ciphertext using hash algorithms. A hashed password cannot be decrypted, but a hacker can try to reverse engineer it. Password salting adds random characters before or after a password prior to hashing to obfuscate the actual password.
/// Source: https://www.pingidentity.com/en/resources/blog/post/encryption-vs-hashing-vs-salting.html
/// </summary>
public class EncryptionContext
{
    /*
    Hash(PasswordEntered + Salt)  = hash in database = authenticated
    Hash(PasswordEntered + Salt)  <> hash in database = not authenticated
    https://stackoverflow.com/questions/41381333/asp-net-identity-where-is-the-salt-stored
    */
    private readonly byte[] _key;
    private readonly byte[] _iv;
    private readonly ConfigurationProvider configurationProvider;
    private readonly UTF8Encoding UTFEncoder;

    public EncryptionContext(ConfigurationProvider configurationProvider)
    {
        this.configurationProvider = configurationProvider;

        _key = Convert.FromBase64String(configurationProvider.GetEncryptionKeyValue);
        _iv = Convert.FromBase64String(configurationProvider.GetInitVectorValue);

        // Create the Aes object safely
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        UTFEncoder = new UTF8Encoding();
    }

    public byte[] Encrypt(string textValue)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            byte[] bytes = UTFEncoder.GetBytes(textValue);
            cryptoStream.Write(bytes, 0, bytes.Length);
        } // cryptoStream is automatically closed and flushed here

        return memoryStream.ToArray();
    }


    public string Decrypt(byte[] encryptedValue)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(encryptedValue);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var resultStream = new MemoryStream();

        cryptoStream.CopyTo(resultStream);

        return UTFEncoder.GetString(resultStream.ToArray());
    }



    public string OneWayHash(string Password)
    {
        using var sha = SHA256.Create();
        var asBytes = Encoding.UTF8.GetBytes(Password);
        var hashed = sha.ComputeHash(asBytes);
        return Convert.ToBase64String(hashed);
    }
}