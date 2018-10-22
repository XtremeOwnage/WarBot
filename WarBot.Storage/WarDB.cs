using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Storage.Models;
using WarBot.Storage.Models.Voting;

namespace WarBot.Storage
{
    public partial class WarDB : DbContext
    {
        public DbSet<Poll> Polls { get; set; }
        public DbSet<DiscordGuild> Guilds { get; set; }
        public DbSet<DiscordUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=warbot.db");
            optionsBuilder.UseLazyLoadingProxies(true);
        }

        public async Task Migrate()
        {
            //Perform migrations as needed.
            var Migrations = await this.Database.GetPendingMigrationsAsync();
            if (Migrations.Count() == 0)
            {
                await Console.Out.WriteLineAsync("WarDB is up to date.");
                return;
            }
            foreach (var m in Migrations)
            {
                await Console.Out.WriteLineAsync("WarDB Migration Pending: " + m);
            }
            await Console.Out.WriteLineAsync("Applying Migrations");

            await this.Database.MigrateAsync();
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
