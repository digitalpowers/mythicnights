using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace MythicNights
{
    internal class JoinChannelCommand : ICommand
    {
        private readonly GuildManager guildManager;

        public JoinChannelCommand(GuildManager guildManager)
        {
            this.guildManager = guildManager;
        }
        public string Command => "!mythic-nights-join";

        public async Task<Response> CommandReceived(string[] parameters, IUser user, SocketUserMessage message)
        {
            if (message.MentionedChannels.FirstOrDefault() is SocketGuildChannel socketGuildChannel)
            {
                ulong guildId = socketGuildChannel.Guild.Id;
                ulong channelId = socketGuildChannel.Id;
                var guildConfig = await guildManager.GetGuildConfig(guildId);
                await guildManager.SetChannel(guildConfig, channelId);
                return Response.Success("bot channel changed");
            }
            return Response.Failulre("cant set bot channel to direct message or without a mention");
        }

        public string Help()
        {
            return $"expects a channel mention for a channel to use.  example: {Command} #a-very-special-channel-for-you";
        }
    }
}
