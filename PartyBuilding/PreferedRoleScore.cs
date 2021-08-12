using MythicNights.DataContext;
using System.Linq;

namespace MythicNights.PartyBuilding
{
    internal class PreferedRoleScore : IScoreGroup
    {
        public int ScoreGroup(Group group)
        {
            int score = group.ToDict().Sum(f=> PrefersRole(f.Item2, f.Item1) ? 1 : 0);

            return score;
        }

        private bool PrefersRole(Player player, Role role)
        {
            return player.Toons.Any(t => t.PreferedRole == role);
        }
    }

}
