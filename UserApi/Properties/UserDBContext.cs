using Microsoft.EntityFrameworkCore;
using UserApi.Properties;

public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { 
                Database.EnsureCreated();
            }

        public DbSet<User> Users => Set<User>();


    }