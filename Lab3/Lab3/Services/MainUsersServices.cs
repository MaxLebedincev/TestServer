using Lab3.Models;
using Lab3.Data;
using System.Linq;
using System;

namespace Lab3.Services
{
    public class MainUsersServices
    {
        private ApplicationContext db; //ApplicationContext db = new ApplicationContext()
        public MainUsersServices(ApplicationContext context)
        {
            db = context;
        }

        public MainUsers Add(string login, string email, string role, string password, DateTime registerDate, DateTime updateDate)
        {
            MainUsers user = new MainUsers { login = login, email = email, role = role, password = password, registerDate = registerDate, updateDate = updateDate };

            db.MainUsers.Add(user);
            db.SaveChanges();

            return user;
        }

        public MainUsers GetByLogin(string login)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.login == login);

            return user;
        }

        public MainUsers GetByEmail(string email)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.email == email);

            return user;
        }

        public MainUsers GetById(int id)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.id == id);

            return user;
        }

        public MainUsers GetByPassword(string password)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.password == password);

            return user;
        }

        public MainUsers GetByUsers(string login, string email)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.login == login || x.email == email);

            return user;
        }

        public MainUsers CheckUser(string login, string password)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.login == login && x.password == password);

            return user;
        }

        public MainUsers Delete(int id)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.id == id);

            if (user != null)
            {
                db.MainUsers.Remove(user);
                db.SaveChanges();

                return user;
            }

            return null;
        }

        public MainUsers Edit(string login, string newLogin)
        {
            var user = db.MainUsers.FirstOrDefault(x => x.login == login);

            if (user != null && GetByLogin(newLogin) == null)
            {

                user.login = newLogin;

                db.MainUsers.Update(user);
                db.SaveChanges();

                return user;
            }

            new Exception("Такой логин занят");
            return null;
        }

    }
}
