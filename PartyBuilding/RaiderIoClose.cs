using MythicNights.DataContext;
using System;
using System.Linq;

namespace MythicNights.PartyBuilding
{
    internal class RaiderIoClose : IScoreGroup
    {
        public int ScoreGroup(Group group)
        {
            double result = 0;
            var players = group.ToList().Select(p => p.Toons.Max(t => t.RaiderIO)).ToList();
            double average = players.Average();
            double sum = players.Sum(d => Math.Pow(d - average, 2));
            result = Math.Sqrt((sum) / (players.Count() - 1));

            return result < 300 ? 10 : 0;
        }
    }

}
