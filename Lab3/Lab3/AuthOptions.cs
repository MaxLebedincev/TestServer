using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace InTheShadowsAPI
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:3000"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 15; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
