using System;
using System.Text;

public class Protector
{
    public static byte[] Key = Guid.NewGuid().ToByteArray();

    public static string Encrypt(string value)
    {
        return Convert.ToBase64String(Encode(Encoding.UTF8.GetBytes(value), Key) );
    }
    
    public static T Decrypt<T>(string value, byte[] key = null)
    {
        return (T)Convert.ChangeType( Encoding.UTF8.GetString(Encode(Convert.FromBase64String(value), Key)), typeof(T) );
    }

    private static byte[] Encode(byte[] bytes, byte[] key)
    {
        var j = 0;

        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key[j];

            if (++j == key.Length)
            {
                j = 0;
            }
        }

        return bytes;
    }
}