using Microsoft.EntityFrameworkCore;
using ToDoCore.Model;

namespace ToDoCore.Adaptors.Db
{
    public class ToDoContext : DbContext
    {
        public DbSet<ToDoItem> ToDoItems {get; set; }

        protected ToDoContext() {}

        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }

            // Fixes issue with MySql connector reporting nested transactions not supported https://github.com/aspnet/EntityFrameworkCore/issues/7017
            //Database.AutoTransactionsEnabled = false;

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