using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
    public class HelloAppContext : DbContext
    {
        public HelloAppContext(DbContextOptions<HelloAppContext> options) : base(options)
        {
        }
        public virtual DbSet<Entity.UserEntity> UserEntities { get; set; }
    }
}
