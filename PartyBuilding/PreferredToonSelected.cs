using MythicNights.DataContext;
using System.Collections.Generic;
using System.Linq;

namespace MythicNights.PartyBuilding
{
    internal class PreferredToonSelected : IScoreGroup
    {
        public int ScoreGroup(Group group)
        {
            int score = 0;
            var dict = group.ToDict();
            foreach (var kv in dict)
            {
                var toonsForRole = ToonForRole(kv.Item2, kv.Item1);
                var preferedToon = PreferredToon(kv.Item2);
                if(toonsForRole.Contains(preferedToon))
                {
                    score += 5;
                }
            }
            return score;
        }
        private List<Toon> ToonForRole(Player player, Role role)
        {
            var toons = player.Toons.Where(t => t.PreferedRole == role).ToList();
            if(toons.Count == 0)
            {
                toons.AddRange(player.Toons.Where(t => t.Offspec == role));
            }
            return toons;
        }

        private Toon PreferredToon(Player player)
        {
            player.Toons.Sort((x, y) =>
            {
                var preferenceDescriminatorX = x.iLvl * 10 + x.RaiderIO;
                var preferenceDescriminatorY = y.iLvl * 10 + y.RaiderIO;
                return preferenceDescriminatorY.CompareTo(preferenceDescriminatorX);
            });
            return player.Toons.First();
        }
    }

}
