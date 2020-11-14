using System;

namespace Guting.Data
{
    public static class Tools
    {
        public static string GetUniqueId()
        {
            var guid = Guid.NewGuid();
            var bytes = guid.ToByteArray();
            var encoded = Convert.ToBase64String(bytes);
            encoded = encoded.Replace('/', '_');
            encoded = encoded.Replace('+', '-');
            encoded = encoded.Substring(0, 22);
            return encoded;
        }
    }
}