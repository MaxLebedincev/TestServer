using Dapper;
using InTheShadowsAPI.Data;
using InTheShadowsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace InTheShadowsAPI.Controllers.Entity
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : UserController
    {
        public TeacherController(ApplicationContext context, IConfiguration conf, IHttpContextAccessor contextAccessor) : base(context, conf, contextAccessor)
        {
        }

        //private SqlConnection _conn;
        //private ApplicationContext db;

        //public TeacherController(ApplicationContext context, IConfiguration conf)
        //{
        //    _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
        //    db = context;
        //    _conn.Open();
        //}

        [Authorize(Roles = "teacher")]
        [Route("deleteCourseTeacher")]
        public JsonResult DeleteCourseTeacher([FromBody] AllCourses course)
        {
            int countRows = 0;

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE [dbo].[AllCourses] WHERE id_author = @id_author AND id = @id", _conn);

                command.Parameters.AddWithValue("@id", course.id);
                command.Parameters.AddWithValue("@id_author", user.id);

                countRows = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return (countRows == 0) ? new JsonResult(new { success = "Курс не найден!" }) : new JsonResult(new { success = "Курс удален!" });
        }

        [Authorize(Roles = "teacher")]
        [Route("insertCourseTeacher")]
        public JsonResult InsertCourseTeacher([FromBody] AllCourses course)
        {

            try
            {
                SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[AllCourses]  (id_author, name, description) VALUES (@id_author, @name, @description)", _conn);

                command.Parameters.AddWithValue("@id_author", user.id);
                command.Parameters.AddWithValue("@name", course.name);
                command.Parameters.AddWithValue("@description", course.description);

                command.ExecuteReader();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(new { success = "Курс добавлен!" });
        }

        [Authorize(Roles = "teacher")]
        [Route("getMyCourses")]
        public async Task<JsonResult> GetMyCourses()
        {
            IEnumerable<AllCourses> courses = new List<AllCourses>();

            try
            {
                courses = await _conn.QueryAsync<AllCourses>(@"SELECT AL.* 
                                                            FROM [dbo].[AllCourses] AL
                                                            WHERE AL.id_author = @id", new { id = user.id });
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(courses);
        }
    }
}
