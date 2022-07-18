using System;

namespace InTheShadowsAPI.Models
{
    public class MainUsers
    {
        public int id { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string password { get; set; }
        public DateTime registerDate { get; set; }
        public DateTime updateDate { get; set; }
    }
}
