using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Data.Models;

namespace WarBot.Data;
public partial class WarDB : DbContext
{
    public DbSet<GuildSettings> GuildSettings { get; set; }

    public WarDB() : base() { }
    public WarDB(Microsoft.EntityFrameworkCore.DbContextOptions options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        MySqlConnector.MySqlConnectionStringBuilder conStrBldr = new MySqlConnector.MySqlConnectionStringBuilder
        {
            Database = BotConfig.DB_NAME ?? "WarBOT",
            Password = BotConfig.DB_PASS ?? "BaconNBlood4Breakfast",
            UserID = BotConfig.DB_USER ?? "WarBOT_User",
            Port = BotConfig.DB_PORT,
            Server = BotConfig.DB_HOST ?? "10.100.5.100",
        };

        var Ver = ServerVersion.AutoDetect(conStrBldr.ConnectionString);
        optionsBuilder.UseMySql(conStrBldr.ConnectionString, Ver);
        //optionsBuilder.UseLazyLoadingProxies(true);
        //this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    public void Migrate()
    {
        //Perform migrations as needed.
        var Migrations = Database.GetPendingMigrations();
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

        Database.Migrate();

    }

    public async Task SaveWithOutput()
    {
        try
        {
            await SaveChangesAsync();
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

public class WarDBDesignTimeFactory : IDesignTimeDbContextFactory<WarDB>
{
    public WarDB CreateDbContext(string[] args)
    {
        return new WarDB();
    }
}
