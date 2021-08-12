using MythicNights.DataContext;
using System;
using System.Linq;

namespace MythicNights.PartyBuilding
{
    internal class IlvlsClose : IScoreGroup
    {
        public int ScoreGroup(Group group)
        {
            double result = 0;
            var players = group.ToList().Select(p=>p.Toons.Max(t=>t.iLvl)).ToList();
            double average = players.Average();
            double sum = players.Sum(d => Math.Pow(d - average, 2));
            result = Math.Sqrt((sum) / (players.Count() - 1));

            return result < 4 ? 2 :0;
        }
    }

}
