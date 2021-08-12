using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MythicNights.DataContext
{
    public class MythicNightContext : DbContext
    {
        public MythicNightContext(DbContextOptions<MythicNightContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //var splitStringConverter = new ValueConverter<List<ulong>, string>(v => string.Join(";", v.ToString()), v => v.Split(new[] { ';' }).Select(u => ulong.Parse(u)).ToList());
            //builder.Entity<MythicNight>().Property(nameof(MythicNight.Attending)).HasConversion(splitStringConverter);
            var converter = new ValueConverter<List<ulong>, string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => ulong.Parse(val)).ToList());
            builder.Entity<MythicNight>().Property(nameof(MythicNight.Attending)).HasConversion(converter);

        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Toon> Toons { get; set; }
        public DbSet<MythicNight> MythicNights { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }
    }
}
