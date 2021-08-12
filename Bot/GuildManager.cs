using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MythicNights.DataContext;
namespace MythicNights
{
    internal class GuildManager
    {
        private readonly IDbContextFactory<MythicNightContext> mContextFactory;

        public GuildManager(IDbContextFactory<MythicNightContext> contextFactory)
        {
            mContextFactory = contextFactory;
        }

        public async Task<GuildConfig> GetGuildConfig(ulong guildId)
        {
            using var context = mContextFactory.CreateDbContext();
            var guildConfig = await context.GuildConfigs.AsQueryable().FirstOrDefaultAsync(p => p.GuildId == guildId, cancellationToken: CancellationToken.None);
            if(guildConfig == null)
            {
                guildConfig = new GuildConfig() { GuildId = guildId };
                context.Add(guildConfig);
                await context.SaveChangesAsync();
            }
            return guildConfig;
        }
        public async Task SetChannel(GuildConfig config, ulong channelId)
        {
            using var context = mContextFactory.CreateDbContext();
            config.ChannelId = channelId;
            context.Update(config);
            await context.SaveChangesAsync();
        }

  
    }
}
