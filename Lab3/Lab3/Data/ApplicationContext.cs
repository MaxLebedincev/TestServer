using InTheShadowsAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace InTheShadowsAPI.Data
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<MainUsers> MainUsers { get; set; }

    }
}
