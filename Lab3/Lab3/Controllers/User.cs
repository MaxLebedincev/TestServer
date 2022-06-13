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

namespace Lab3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private SqlConnection _conn;
        private ApplicationContext db;

        public UserController(ApplicationContext context, IConfiguration  conf)
        {
            _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
            db = context;
            _conn.Open();
        }

        public class Course
        {
            public int id_course { get; set; }
            public int id_student { get; set; }
        }

        [Authorize]
        [Route("getinfo")]
        public JsonResult GetInfo()
        {
            var user = new MainUsersServices(db).GetByLogin(User.Identity.Name);

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
