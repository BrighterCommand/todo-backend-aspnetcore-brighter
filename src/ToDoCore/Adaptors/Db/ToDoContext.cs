using Microsoft.EntityFrameworkCore;
using ToDoCore.Model;

namespace ToDoCore.Adaptors.Db
{
    public class ToDoContext : DbContext
    {
        protected ToDoContext()
        {
        }

        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
        }

        public DbSet<ToDoItem> ToDoItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoItem>()
                .Property(field => field.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ToDoItem>()
                .HasKey(field => field.Id);
        }
    }
}