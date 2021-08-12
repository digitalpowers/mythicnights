using Microsoft.Extensions.Logging;
using MythicNights.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MythicNights.PartyBuilding
{
    internal class PartyBuilder
    {
        private readonly Random mRandom;
        private readonly List<IScoreGroup> mGroupScorers;
        private readonly ILogger<PartyBuilder> logger;

        public PartyBuilder(IEnumerable<IScoreGroup> groupScorers, ILogger<PartyBuilder> logger)
        {
            mRandom = new Random();
            mGroupScorers = groupScorers.ToList();
            this.logger = logger;
        }
        public List<Group> BuildParty(List<Player> players)
        {
            if(players.Count == 0)
            {
                return new List<Group>();
            }
            foreach(var p in players)
            {
                logger.LogInformation($"Player '{p.DiscordUserId}', Nick: {p.Nickname}, Toons: {p.Toons.Count}");
                if(p.Toons.Count > 0)
                {
                    foreach (Toon t in p.Toons)
                    {
                        logger.LogInformation($"Toon '{t.FullName}', RIO: {t.RaiderIO}, Ilvl: {t.iLvl}, Main: {t.PreferedRole}, Off: {t.Offspec}");
                    }
                }
            }
            players = players.Where(p => p.Toons.Count > 0).ToList();
            List<List<Group>> groupMetaList = new List<List<Group>>();
            for (int i = 0; i < 100; i++)
            {
                var playersClone = new List<Player>(players);

                var groups = new List<Group>();
                Group group = null;
                while (playersClone.Count > 0)
                {
                    group = new Group()
                    {
                        Tank = FindRole(playersClone, Role.Tank),
                        Healer = FindRole(playersClone, Role.Healer),
                        Dps1 = FindRole(playersClone, Role.Dps),
                        Dps2 = FindRole(playersClone, Role.Dps),
                        Dps3 = FindRole(playersClone, Role.Dps),
                    };
                    groups.Add(group);
                    if(group.ToList().Count == 0)
                    {
                        break;
                    }
                }
                groupMetaList.Add(groups);
            }
            List<(int, List<Group>)> scoredList = new List<(int, List<Group>)>();
            foreach(List<Group> bigGroups in groupMetaList)
            {
                int fullScore = 0;
                foreach(var group in bigGroups)
                {
                    var score = mGroupScorers.Sum(f => f.ScoreGroup(group));
                    fullScore += score;
                }
                scoredList.Add((fullScore, bigGroups));
            }
            scoredList.Sort((x, y) => y.Item1.CompareTo(x.Item1));

            return scoredList.First().Item2;
        }
        public Player FindRole(List<Player> players, Role role)
        {
            var optionsMain = players.Where(f => {
                return f.Toons.Any(t => t.PreferedRole == role);
            }).ToList();
            var optionsAlt = players.Where(f => {
                return f.Toons.Any(t => t.Offspec == role);
            }).ToList();
            var options = new List<Player>();
            options.AddRange(optionsMain);
            options.AddRange(optionsMain);
            options.AddRange(optionsMain);
            options.AddRange(optionsAlt);
            if (options.Count == 0)
            {
                return null;
            }
            var player = options[mRandom.Next(0, options.Count - 1)];
            players.Remove(player);
            return player;
        }

    }
}
