﻿using Dapper;
using Lab3.Data;
using Lab3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Lab3.Controllers.Entity
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private SqlConnection _conn;
        private ApplicationContext db;

        public AdminController(ApplicationContext context, IConfiguration conf)
        {
            _conn = new SqlConnection(conf.GetConnectionString("DefaultConnection"));
            db = context;
            _conn.Open();
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
            int countRows = 0;

            try
            {
                SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[MainUsers] WHERE id = @id AND 'admin' != @role", _conn);

                command.Parameters.AddWithValue("@id", user.id);
                command.Parameters.AddWithValue("@role", user.role);

                countRows = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Произошла ошибка!" });
            }

            return (countRows == 0) ? new JsonResult(new { success = "Пользователь не найден!" }) : new JsonResult(new { success = "Пользователь удален!" });
        }
    }
}