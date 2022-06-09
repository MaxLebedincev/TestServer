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

            return new JsonResult(new {success = "Курс добавлен!"});
        }

        [Authorize(Roles = "student")]
        [Route("deleteCourse")]
        public  JsonResult DeleteCourse([FromBody] Course course)
        {

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[SubscriptionCourses] WHERE id_author = @id_author AND id_user = @id_user)", _conn);

                command.Parameters.AddWithValue("@id_author", course.id_course);
                command.Parameters.AddWithValue("@id_user", course.id_student);

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(new { success = "Курс удален!" });
        }

        [Authorize(Roles = "teacher")]
        [Route("deleteCourseЕeacher")]
        public JsonResult DeleteCourseTeacher([FromBody] AllCourses course)
        {

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[AllCourses] WHERE id_author = @id_author AND id = @id)", _conn);

                command.Parameters.AddWithValue("@id", course.id);
                command.Parameters.AddWithValue("@id_author", course.id_author);

                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(new { success = "Курс удален!" });
        }

        [Authorize(Roles = "teacher")]
        [Route("insertCourseЕeacher")]
        public JsonResult InsertCourseTeacher([FromBody] AllCourses course)
        {

            try
            {
                SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[SubscriptionCourses] (id, id_author, name, description) VALUES (@id, @id_author, @name, @description)", _conn);

                command.Parameters.AddWithValue("@id", course.id);
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

        [Authorize(Roles = "admin")]
        [Route("getAllUser")]
        public async Task<JsonResult> GetAllUser()
        {

            var courses = await _conn.QueryAsync<MainUsers>("SELECT * FROM MainUsers");

            return new JsonResult(courses);
        }

        [Authorize(Roles = "admin")]
        [Route("deleteUser")]
        public JsonResult DeleteUser([FromBody] MainUsers user)
        {

            try
            {
                _conn.Open();

                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[MainUsers] WHERE id = @id AND 'admin' != @role", _conn);

                command.Parameters.AddWithValue("@id", user.id);
                command.Parameters.AddWithValue("@role", user.role);

                command.ExecuteReader();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(new { success = "Пользователь удален!" });
        }
    }
}
