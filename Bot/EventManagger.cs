using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.EntityFrameworkCore;
using MythicNights.DataContext;
using MythicNights.PartyBuilding;

namespace MythicNights
{
    internal class EventManager
    {
        private readonly IDbContextFactory<MythicNightContext> mContextFactory;
        private readonly PlayerManager playerManager;
        private readonly PartyBuilder partyBuilder;

        public EventManager(IDbContextFactory<MythicNightContext> contextFactory, PlayerManager playerManager, PartyBuilder partyBuilder)
        {
            mContextFactory = contextFactory;
            this.playerManager = playerManager;
            this.partyBuilder = partyBuilder;
        }

        public async Task<MythicNight> CreateEvent(ulong eventId, DateTime time)
        {
            using var context = mContextFactory.CreateDbContext();
            var mythicNight = new MythicNight()
            {
                Id = eventId,
                EventTime = time,
                Attending = new List<ulong>(),
            };
            context.Add(mythicNight);
            await context.SaveChangesAsync();
            return mythicNight;
        }

        public async Task<MythicNight> GetEvent(ulong eventId)
        {
            using var context = mContextFactory.CreateDbContext();
            var mythicNight = await context.MythicNights.AsQueryable().FirstOrDefaultAsync(p => p.Id == eventId);
            return mythicNight;
        }

        public async Task AddAttendie(ulong eventId, IUser user)
        {
            if (await GetEvent(eventId) is MythicNight mythicNight)
            {
                var player = await playerManager.GetPlayer(user.Id, user.Username);
                await playerManager.SendDM(player, "welcome, add toons by doing !add name-realm, more info with !help");
                using var context = mContextFactory.CreateDbContext();
                mythicNight.Attending.Add(player.DiscordUserId);
                context.Update(player);
                context.Update(mythicNight);
                await context.SaveChangesAsync();
            }
        }

        internal async Task<List<Group>> CreateGroups(ulong eventId)
        {
            if (await GetEvent(eventId) is MythicNight mythicNight)
            {
                using var context = mContextFactory.CreateDbContext();
                var players = context.Players.Include(p=>p.Toons).Where(p => mythicNight.Attending.Contains(p.DiscordUserId)).ToList();
                return partyBuilder.BuildParty(players);
            }
            return null;
        }

        internal async Task<List<Group>> CreateGroups(List<ulong> playerIds)
        {
            using var context = mContextFactory.CreateDbContext();
            var players = await context.Players.Include(p => p.Toons).Where(p => playerIds.Contains(p.DiscordUserId)).ToListAsync();
            return partyBuilder.BuildParty(players);
        }

        internal async Task RemoveAttendie(ulong eventId, IUser user)
        {
            if (await GetEvent(eventId) is MythicNight mythicNight)
            {
                var player = await playerManager.GetPlayer(user.Id);
                await playerManager.SendDM(player, $"You are no longer attending the event on {mythicNight.EventTime}");
                using var context = mContextFactory.CreateDbContext();
                mythicNight.Attending.Remove(player.DiscordUserId);
                context.Update(player);
                context.Update(mythicNight);
                await context.SaveChangesAsync();
            }
        }

        internal async Task<List<MythicNight>> GetEvents()
        {
            using var context = mContextFactory.CreateDbContext();
            var events = await context.MythicNights.AsQueryable().ToListAsync();
            return events;
        }
    }
}
