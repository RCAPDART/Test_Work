using System;
using System.Security.Cryptography;
using System.Text;

namespace Test_Work.Helpers
{
    public class UrlMinificationHelper : IUrlMinificationHelper
    {
        public string UrlMinificate(string longUrl, bool withSeed = true)
        {
            if (withSeed)
            {
                var seed = DateTime.Now; // Ticks are more suitable for seed.
                longUrl = $"{longUrl}{seed.ToString("o")}";
            }

            int symbolCount = 4; // From config? Why int, not var? You are using in some places var and in some places classNames.
            int startindex = new Random().Next(0, 31 - (symbolCount + 1));

            return GetShortHash(url: longUrl,startindex: startindex, symbolCount: symbolCount);
        }

        // Public with private methods are meshed.
        private static string GetSHA256(string originalUrl)
        {
            try
            {
                var hasher = new SHA256Managed();

                byte[] urlBytes = new UTF8Encoding().GetBytes(originalUrl);
                byte[] keyBytes = hasher.ComputeHash(urlBytes);

                hasher.Dispose();
                return Convert.ToBase64String(keyBytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static string GetShortHash(string url, int startindex = 0, int symbolCount = 4)
        {
            if (url == null || string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            string hash = GetSHA256(url);

            return hash.Substring(startindex, symbolCount);
        }
    }
}
