using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MythicNights
{
    internal class CreateEventCommand : ICommand
    {
        private readonly PlayerManager playerManager;
        private readonly GuildManager guildManager;
        private readonly DiscordSocketClient client;
        private readonly EventManager eventManager;

        public CreateEventCommand(PlayerManager playerManager, GuildManager guildManager, DiscordSocketClient client, EventManager eventManager) 
        {
            this.playerManager = playerManager;
            this.guildManager = guildManager;
            this.client = client;
            this.eventManager = eventManager;
        }
        public string Command => "!mythic-nights-create-event";

        public async Task<Response>  CommandReceived(string[] parameters, IUser user, SocketUserMessage message)
        {
            var dateString = string.Join(" ", parameters);
            var channel = message.Channel as SocketGuildChannel;
            if(channel == null)
            {
                return Response.Failulre($"Events must be created in a channel, not in direct message");
            }
            var guildConfig = await guildManager.GetGuildConfig(channel.Guild.Id);
            if(!guildConfig.ChannelId.HasValue)
            {
                return Response.Failulre($"set a channel to work in with !join #channelname");
            }
            if (DateTime.TryParse(dateString, out var time))
            {
                var msgChannel = client.GetChannel(channel.Id) as IMessageChannel;
                var msg = await msgChannel.SendMessageAsync($"Mythic Night! at {time}");
                var attending = new Emoji("👍");
                await msg.AddReactionAsync(attending);
                var tryGroups = new Emoji("😄");
                await msg.AddReactionAsync(tryGroups);

                await eventManager.CreateEvent(msg.Id, time);
                return Response.Success("created");
            }
            return Response.Failulre("unknown error ... maybe date parsing?");
        }

        public string Help()
        {
            return $"expects a date/time to create the event for. example: {Command} 9/10/21 8:45PM";
        }
    }
}
