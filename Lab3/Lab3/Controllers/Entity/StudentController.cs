using Lab3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using static Lab3.Controllers.UserController;

namespace Lab3.Controllers.Entity
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private SqlConnection _conn;
        private ApplicationContext db;

        public StudentController(ApplicationContext context, IConfiguration conf)
        {
            _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
            db = context;
            _conn.Open();
        }

        [Authorize(Roles = "student")]
        [Route("setCourse")]
        public JsonResult SetCourse([FromBody] Course course)
        {

            try
            {
                SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[SubscriptionCourses] (id_author, id_user) VALUES (@id_author, @id_user)", _conn);

                command.Parameters.AddWithValue("@id_author", course.id_course);
                command.Parameters.AddWithValue("@id_user", course.id_student);

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(new { success = "Курс добавлен!" });
        }

        [Authorize(Roles = "student")]
        [Route("deleteCourse")]
        public JsonResult DeleteCourse([FromBody] Course course)
        {
            int countRows = 0;

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[SubscriptionCourses] WHERE id_author = @id_author AND id_user = @id_user)", _conn);

                command.Parameters.AddWithValue("@id_author", course.id_course);
                command.Parameters.AddWithValue("@id_user", course.id_student);

                countRows = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return (countRows == 0) ? new JsonResult(new { success = "Курс не найден!" }) : new JsonResult(new { success = "Курс удален!" });
        }
    }
}
