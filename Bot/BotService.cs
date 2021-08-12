using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MythicNights.DataContext;

namespace MythicNights
{
    internal class BotService : IHostedService
    {
        Dictionary<string, ICommand> mCommands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
        public BotService(
            IEnumerable<ICommand> commands, 
            GuildManager guildManager, 
            DiscordSocketClient client, 
            EventManager eventManager, 
            IConfiguration config,
            ILogger<BotService> logger)
        {
            foreach(var command in commands)
            {
                mCommands.Add(command.Command, command);
            }

            this.guildManager = guildManager;
            _client = client;
            this.eventManager = eventManager;
            this.config = config;
            this.logger = logger;
        }

        DiscordSocketClient _client;
        private readonly EventManager eventManager;
        private readonly IConfiguration config;
        private readonly ILogger<BotService> logger;
        private readonly GuildManager guildManager;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Log += async (message) =>
            {
                Console.WriteLine(message.Message);
            };

            _client.ReactionAdded += async (msg, chanel, reaction) =>
            {
                try
                {
                    if (reaction.User.Value.IsBot)
                    {
                        return;
                    }
                    if (reaction.Emote.Name == "👍")
                    {
                        await eventManager.AddAttendie(msg.Id, reaction.User.Value);
                    }
                    else if (reaction.Emote.Name == "😄")
                    {
                        var groups = await eventManager.CreateGroups(msg.Id);
                        await chanel.SendMessageAsync("\ntank/heals/dps1/dps2/dps3\n" + string.Join("\n", groups));
                    }
                }
                catch(Exception e)
                {
                    logger.LogError(e, $"exception during ReactionAdded");
                }
            };

            _client.ReactionRemoved += async (msg, chanel, reaction) =>
            {
                if (reaction.Emote.Name == "👍")
                {
                    await eventManager.RemoveAttendie(msg.Id, reaction.User.Value);
                }
            };

            _client.MessageReceived += async (messageParam) =>
            {
                var message = messageParam as SocketUserMessage;
                int argPos = 0;

                // Determine if the message is a command based on the prefix and make sure no bots trigger commands
                if (!(message.HasCharPrefix('!', ref argPos) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                    message.Author.IsBot)
                    return;

                if(message.Channel is SocketGuildChannel channel)
                {
                    GuildConfig guildConfig = await guildManager.GetGuildConfig(channel.Guild.Id);
                    if(guildConfig.ChannelId != null && channel.Id != guildConfig.ChannelId.Value)
                    {
                        return;
                    }
                }

                var parsedMsg = message.Content.Split(" ");
                var cmd = parsedMsg.First();
                var parameters = parsedMsg.Skip(1).ToArray();
                if(mCommands.ContainsKey(cmd))
                {
                    try
                    {
                        var resp = await mCommands[cmd].CommandReceived(parameters, message.Author, message);
                        await message.ReplyAsync(resp.Message);
                    }
                    catch(Exception e)
                    {
                        await message.ReplyAsync($"failed with exception {e}");
                    }
                }
                else if(cmd == "!help" && parameters.Length==0)
                {
                    await message.ReplyAsync($"available commands: \n{string.Join("\n", mCommands.Keys)}");
                }
                else if (cmd == "!help" && parameters.Length == 1 && mCommands.ContainsKey(parameters[0]))
                {
                    var resp = mCommands[parameters[0]].Help();
                    await message.ReplyAsync(resp);
                }
            };

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = config["Discord:BotSecret"]; 

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            

            //foreach(var guild in _client.Guilds)
            //{
            //    var config = await guildManager.GetGuildConfig(guild.Id);
            //    GuildConfigs[guild.Id] = config;
            //}
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
