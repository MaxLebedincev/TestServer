using Dapper;
using Lab3.Data;
using Lab3.Models;
using Lab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lab3.Controllers.Entity
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : UserController
    {
        public StudentController(ApplicationContext context, IConfiguration conf, IHttpContextAccessor contextAccessor) : base(context, conf, contextAccessor)
        {
        }

        //private SqlConnection _conn;
        //private ApplicationContext db;
        //private MainUsers user;

        //public StudentController(ApplicationContext context, IConfiguration conf)
        //{
        //    _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
        //    db = context;
        //    _conn.Open();
        //    user = new MainUsersServices(db).GetByLogin(User.Identity.Name);
        //}

        public class Course
        {
            public int id_course { get; set; }
        }

        [Authorize(Roles = "student")]
        [Route("setCourse")]
        public async Task<JsonResult> SetCourse([FromBody] Course course)
        {
            IEnumerable<AllCourses> courses = null;

            try
            {
                courses = await _conn.QueryAsync<AllCourses>(@"SELECT * FROM [dbo].[SubscriptionCourses] WHERE id_course = @id_course AND id_student = @id_student",
                    new { 
                        id_course = course.id_course, 
                        id_student = user.id 
                    });
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            if (courses.ToList().Count != 0) return new JsonResult(new { success = "Курс уже добавлен!" });

            try
            {
                SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[SubscriptionCourses] (id_course, id_student) VALUES (@id_course, @id_student)", _conn);

                command.Parameters.AddWithValue("@id_course", course.id_course);
                command.Parameters.AddWithValue("@id_student", user.id);

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
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[SubscriptionCourses] WHERE id_course = @id_course AND id_student = @id_student", _conn);

                command.Parameters.AddWithValue("@id_course", course.id_course);
                command.Parameters.AddWithValue("@id_student", user.id);

                countRows = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return (countRows == 0) ? new JsonResult(new { success = "Курс не найден!" }) : new JsonResult(new { success = "Курс удален!" });
        }

        [Authorize(Roles = "student")]
        [Route("getMyCourses")]
        public async Task<JsonResult> GetMyCourses()
        {
            IEnumerable<AllCourses> courses = new List<AllCourses>();

            try
            {
                courses = await _conn.QueryAsync<AllCourses>(@"SELECT AL.* 
                                                            FROM[dbo].[AllCourses] AL
                                                            INNER JOIN[dbo].[SubscriptionCourses] SC
                                                            ON AL.id = SC.id_course
                                                            WHERE SC.id_student = @id", new { id = user.id });
            }
            catch (Exception)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return new JsonResult(courses);
        }
    }
}
