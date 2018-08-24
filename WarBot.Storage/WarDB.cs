using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Storage.Models;

namespace WarBot.Storage
{
    public partial class WarDB : DbContext
    {
        public WarDB(DbContextOptions<WarDB> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=warBOT.db")
                .UseLazyLoadingProxies(true);        

        }

        public DbSet<DiscordGuild> Guilds { get; set; }
        public DbSet<DiscordUser> Users { get; set; }

        public async Task Migrate()
        {
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
            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        await Console.Out.WriteLineAsync("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            await Console.Out.WriteLineAsync("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
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
            var optionsBuilder = new DbContextOptionsBuilder<WarDB>();
            optionsBuilder.UseSqlite("Data Source=warBOT.db");

            return new WarDB(optionsBuilder.Options);
        }
    }
}
