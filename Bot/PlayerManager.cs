using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using MythicNights.DataContext;
using MythicNights.RaiderIO;
namespace MythicNights
{
    internal class PlayerManager
    {
        private readonly IDbContextFactory<MythicNightContext> mContextFactory;
        private readonly RaiderIOClient mRioClient;
        private readonly DiscordSocketClient client;

        public PlayerManager(IDbContextFactory<MythicNightContext> contextFactory, RaiderIOClient rioClient, DiscordSocketClient client)
        {
            mContextFactory = contextFactory;
            mRioClient = rioClient;
            this.client = client;
        }

        public async Task<Player> GetPlayer(ulong discordId)
        {
            using var context = mContextFactory.CreateDbContext();
            var player = await context.Players.Include(p => p.Toons).FirstOrDefaultAsync(p => p.DiscordUserId == discordId);
            return player;
        }

        public async Task<Player> GetPlayer(ulong discordId, string discordUsername)
        {
            using var context = mContextFactory.CreateDbContext();
            var player = await context.Players.Include(p => p.Toons).FirstOrDefaultAsync(p => p.DiscordUserId == discordId);
            if(player == null)
            {
                player = new Player()
                {
                    DiscordUserId = discordId,
                    DiscordUsername = discordUsername,
                    Toons = new List<Toon>(),
                    Nickname = discordUsername
                };
                context.Add(player);
                await context.SaveChangesAsync();
            }
            return player;
        }

        public async Task<Response> AddToon(Player player, string name, string realm)
        {
            var stats = await mRioClient.GetCharacterStats(name, realm);
            if(stats.mythic_plus_scores_by_season == null)
            {
                return Response.Failulre("Failed to find toon by that name/realm");
            }
            var scores = new List<(Role, float)>
                {
                    (Role.Dps, stats.mythic_plus_scores_by_season[0].scores.dps ),
                    (Role.Tank, stats.mythic_plus_scores_by_season[0].scores.tank ),
                    (Role.Healer, stats.mythic_plus_scores_by_season[0].scores.healer )
                };
            scores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            var preferedRole = scores[0].Item1;
            var toon = new Toon()
            {
                Name = stats.name,
                Realm = stats.realm,
                iLvl = stats.gear.item_level_equipped,
                PreferedRole = preferedRole,
                RaiderIO = stats.mythic_plus_scores_by_season[0].scores.all
            };
            if (scores.Count > 1 && scores[1].Item2 > 0)
            {
                toon.Offspec = scores[1].Item1;
            }
            using var context = mContextFactory.CreateDbContext();

            context.Toons.Add(toon);
            player.Toons.Add(toon);
            context.Update(player);
            await context.SaveChangesAsync();

            return Response.Success($"added {toon.FullName} with RIO of {toon.RaiderIO}");
        }

        internal async Task SendDM(Player player, string message)
        {
            var user = client.GetUser(player.DiscordUserId);
            await Discord.UserExtensions.SendMessageAsync(user, message);
        }

        internal async Task<Response> SetToonPreference(Player player, string name, string realm, bool? prefer)
        {
            var fullName = $"{name}-{realm}";
            var toon = player.Toons.Where(t => t.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if(toon == null)
            {
                return Response.Failulre($"failed to find toon {fullName}");
            }
            using var context = mContextFactory.CreateDbContext();
            toon.IsPrefered = prefer;
            context.Update(toon);
            context.Update(player);
            await context.SaveChangesAsync();
            string message;
            if(!prefer.HasValue)
            {
                message = $"preferences unset for {fullName}";
            }
            else if(prefer.Value)
            {
                message = $"ok you prefer to play {fullName}";
            }
            else
            {
                message = $"ok you prefer not to play {fullName} (but are willing ... delete the toon if not willing)";
            }
            return Response.Success(message);
        }

        internal async Task<Response> SetMainSpec(Player player, string name, string realm, Role role)
        {
            var fullName = $"{name}-{realm}";
            var toon = player.Toons.Where(t => t.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (toon == null)
            {
                return Response.Failulre($"failed to find toon {fullName}");
            }
            using var context = mContextFactory.CreateDbContext();
            toon.PreferedRole = role;
            context.Update(toon);
            context.Update(player);
            await context.SaveChangesAsync();
            return Response.Success($"ok your main spec for {fullName} is now {role}");
        }

        internal async Task<Response> SetOffSpec(Player player, string name, string realm, Role role)
        {
            var fullName = $"{name}-{realm}";
            var toon = player.Toons.Where(t => t.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (toon == null)
            {
                return Response.Failulre($"failed to find toon {fullName}");
            }
            using var context = mContextFactory.CreateDbContext();
            toon.Offspec = role;
            context.Update(toon);
            context.Update(player);
            await context.SaveChangesAsync();
            return Response.Success($"ok your off spec for {fullName} is now {role}");
        }

        internal async Task<Response> DeleteToon(Player player, string name, string realm)
        {
            var fullName = $"{name}-{realm}";
            var toon = player.Toons.Where(t => t.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (toon == null)
            {
                return Response.Failulre($"failed to find toon {fullName}");
            }
            using var context = mContextFactory.CreateDbContext();
            context.Remove(toon);
            player.Toons.Remove(toon);
            context.Update(player);
            await context.SaveChangesAsync();
            string message;
           
            return Response.Success($"{fullName} has been removed from your roster");
        }

        internal async Task<Response> UpdateToons(Player player)
        {
            using var context = mContextFactory.CreateDbContext();
            foreach(var toon in player.Toons)
            {
                var stats = await mRioClient.GetCharacterStats(toon.Name, toon.Realm);
                if (stats.mythic_plus_scores_by_season == null)
                {
                    continue;
                }
                toon.iLvl = stats.gear.item_level_equipped;
                toon.RaiderIO = Math.Round(stats.mythic_plus_scores_by_season[0].scores.all);
                context.Update(toon);
            }
            context.Update(player);
            await context.SaveChangesAsync();

            return Response.Success($"update complete");
        }

        public async Task<Response> SetNick(ulong id, string username, string nickname)
        {
            var player = await GetPlayer(id, username);
            player.Nickname = nickname;
            using var context = mContextFactory.CreateDbContext();
            context.Update(player);
            await context.SaveChangesAsync();
            return Response.Success($"ok i will call you {nickname}");
        }
    }
}
