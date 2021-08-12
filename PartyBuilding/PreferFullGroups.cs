using MythicNights.DataContext;

namespace MythicNights.PartyBuilding
{
    internal class PreferFullGroups : IScoreGroup
    {
        public int ScoreGroup(Group group)
        {
            //-5 points per missing player
            var players = group.ToList();
            var score =  (players.Count - 5) * 5;
            return score;
        }
    }

}
