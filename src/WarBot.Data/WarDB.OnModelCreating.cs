using Microsoft.EntityFrameworkCore;
using WarBot.Data.Models;

namespace WarBot.Data
{
    public partial class WarDB : DbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GuildSettings>()
                .HasMany(o => o.CustomCommands)
                .WithOne(o => o.Parent);

            builder.Entity<CustomSlashCommand>()
                .HasMany(o => o.Actions)
                .WithOne(o => o.Parent);
        }
    }
}
