using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Lab3.Data;
using Lab3.Models;
using Lab3.Services;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http;

namespace Lab3.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        protected SqlConnection _conn;
        protected ApplicationContext db;
        protected MainUsers user;
        
        public UserController(ApplicationContext context, IConfiguration  conf, IHttpContextAccessor contextAccessor)
        {
            _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
            db = context;
            _conn.Open();
            user = new MainUsersServices(db).GetByLogin(contextAccessor.HttpContext.User.Identity.Name);
        }

        [Authorize]
        [Route("getinfo")]
        public JsonResult GetInfo()
        {
            return new JsonResult(new {
                id = user.id,
                login = user.login,
                email = user.email,
                role = user.role
            });
        }

        [Authorize]
        [Route("getAllCourses")]
        public async Task<JsonResult> GetAllCourses()
        {
            var courses = await _conn.QueryAsync<AllCourses>("SELECT * FROM AllCourses");

            return new JsonResult(courses);
        }
    }
}
