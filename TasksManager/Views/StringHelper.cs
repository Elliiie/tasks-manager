using System.Security.Cryptography;
using System.Text;

class StringHelper
{
    public static string ComputeHash(string text)
    {
        string hashed = "";
        using (SHA512 shaM = new SHA512Managed())
        {
            hashed = GetStringFromHash(shaM.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }
        return hashed;
    }

    private static string GetStringFromHash(byte[] hash)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }
}