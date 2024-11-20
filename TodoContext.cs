using Microsoft.EntityFrameworkCore;

namespace ToDoList
{
    public class TodoContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=todo.db");
        }
    }
}
