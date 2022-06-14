using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Lab3.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Data.SqlClient;
using Lab3.Data;
using System;
using System.Linq;
using Lab3.Security;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Lab3.Services;
using Microsoft.AspNetCore.Http;

namespace Lab3.Controllers
{
    public class AccountController : ControllerBase
    {
        private ApplicationContext db;

        public AccountController(ApplicationContext context)
        {
            db = context;
        }


        [HttpPost("/register")]
        public JsonResult Register([FromBody] MainUsers data)
        {

            if (string.IsNullOrEmpty(data.login) || string.IsNullOrEmpty(data.password) || string.IsNullOrEmpty(data.email) || string.IsNullOrEmpty(data.role))
                return new JsonResult(new { errorText = "Поле не заполненно!" });

            if ( new MainUsersServices(db).GetByUsers(data.login, data.email) == null)
            {

                DateTime date = DateTime.Now;

                data.password = new Security.Security().GetHash(date.ToString() + data.password + date.ToString());

                new MainUsersServices(db).Add(data.login, data.email, data.role, data.password, date, date);

                return new JsonResult(new {
                    success = "Пользователь зарегестрирован!" 
                });
            }
            
            return new JsonResult(new { errorText = "Почта или Логин уже используются." });
        }

        [HttpPost("/token")]
        public JsonResult Token([FromBody] MainUsers data)
        {
            var identity = GetIdentity(data.login, data.password);
            if (identity == null)
            {
                return new JsonResult(new { errorText = "Логин или пароль не подходят." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Response.Cookies.Append("jwt", encodedJwt, new CookieOptions
            {
                //HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            var user = new MainUsersServices(db).GetByLogin(data.login);

            var response = new
            {
                id = user.id,
                login = identity.Name,
                email = user.email,
                role = user.role
            };

            return new JsonResult(response);
        }

        [HttpPost("/logout")]
        public JsonResult Logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(-1),
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return new JsonResult(new { message = "Вы успешно вышли." });
        }

        private ClaimsIdentity GetIdentity(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password)) return null;

            MainUsers supposedUser = new MainUsersServices(db).GetByLogin(login);
            MainUsers verifieduser = null;

            if (supposedUser != null)
                 verifieduser = new MainUsersServices(db).CheckUser(login, new Security.Security().GetHash(supposedUser.registerDate.ToString() + password + supposedUser.updateDate.ToString()));
            
            if (verifieduser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, verifieduser.login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, verifieduser.role)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
    }
}