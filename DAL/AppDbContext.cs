using System.Linq;
using Microsoft.EntityFrameworkCore;
using Models;

namespace DAL
{
    // dotnet ef migrations add game --project DAL --startup-project BattleShip
    // dotnet ef database update --project DAL --startup-project BattleShip
    // dotnet ef database drop --project DAL --startup-project BattleShip
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = "Data Source=/home/jevgeni/RiderProjects/HW/BattleShip/BattleShip/battleship.db";

        public DbSet<DbRecord> Records { get; set; } = default!;

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        
        }

        public AppDbContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // remove the cascade delete globally
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }

}