using Microsoft.EntityFrameworkCore;
using Joallyfer.Models;
namespace Joallyfer
{
    public class AppDataContext : DbContext
    {
        public DbSet<Consumo> Consumos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Barbara_Joallyfer.db");
        }
    }
}
