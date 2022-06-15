using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace RolebaseAccess.Data
{
     public class ApiDbContext : DbContext
    {
        public virtual DbSet<ItemData> Items {get;set;}

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            
        }
    }
}