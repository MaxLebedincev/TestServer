using System;
using System.Text;
using System.Security.Cryptography;

namespace Lab3.Security
{
    public class Security
    {
        public string GetHash(string str)
        {
            //var md5 = MD5.Create();
            //var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            var sha512 = SHA512.Create();
        
            var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(str));

            return Convert.ToBase64String(hash);
        }
    }
}
