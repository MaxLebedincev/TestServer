using Lab3.Data;
using Lab3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace Lab3.Controllers.Entity
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : Controller
    {
        private SqlConnection _conn;
        private ApplicationContext db;

        public TeacherController(ApplicationContext context, IConfiguration conf)
        {
            _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
            db = context;
            _conn.Open();
        }

        [Authorize(Roles = "teacher")]
        [Route("deleteCourseTeacher")]
        public JsonResult DeleteCourseTeacher([FromBody] AllCourses course)
        {
            int countRows = 0;

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[AllCourses] WHERE id_author = @id_author AND id = @id", _conn);

                command.Parameters.AddWithValue("@id", course.id);
                command.Parameters.AddWithValue("@id_author", course.id_author);

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

                command.Parameters.AddWithValue("@id_author", course.id_author);
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
    }
}
