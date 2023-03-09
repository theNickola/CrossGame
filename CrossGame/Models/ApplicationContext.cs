using Microsoft.EntityFrameworkCore;

namespace CrossGame.Models
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Game> Games { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Game>().HasData(new Game { 
            //    Id = 1, 
            //    Status = "End", 
            //    Player1="User1", 
            //    StatePl1="stop", 
            //    Player2="User2", 
            //    StatePl2="stop", 
            //    Step="no", 
            //    Winner="no"
            //});
        }
    }
}