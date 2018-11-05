using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core.Voting;
using WarBot.Storage.Models;

namespace WarBot.Storage
{
    public partial class WarDB : DbContext
    {
        public DbSet<Poll> Polls { get; set; }
        public DbSet<DiscordGuild> Guilds { get; set; }
        public DbSet<DiscordUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Config/warbot.db");
            optionsBuilder.UseLazyLoadingProxies(true);
        }

        public void Migrate()
        {
            //Perform migrations as needed.
            var Migrations = this.Database.GetPendingMigrations();
            if (Migrations.Count() == 0)
            {
                Console.WriteLine("WarDB is up to date.");
                return;
            }
            foreach (var m in Migrations)
            {
                Console.WriteLine("WarDB Migration Pending: " + m);
            }
            Console.WriteLine("Applying Migrations");

            this.Database.Migrate();

        }

        public async Task SaveWithOutput()
        {
            try
            {
                await this.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await Console.Out.WriteLineAsync(ex.InnerException.ToString());
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                throw;
            }
        }
    }

    public class BloggingContextFactory : IDesignTimeDbContextFactory<WarDB>
    {

        public WarDB CreateDbContext(string[] args)
        {
            return new WarDB();
        }
    }
}
