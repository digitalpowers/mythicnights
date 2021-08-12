using System.Collections.Generic;
using System.Linq;
using MythicNights.DataContext;

namespace MythicNights
{
    internal static class Extentions
    {
        public static List<Player> ToList(this Group group)
        {
            return new List<Player> { group.Tank, group.Healer, group.Dps1, group.Dps2, group.Dps3 }.Where(p=>p!=null).ToList();
        }
        public static List<(Role, Player)> ToDict(this Group group)
        {
            return new List<(Role, Player)> { 
                ( Role.Tank, group.Tank ), 
                ( Role.Healer, group.Healer ), 
                ( Role.Dps, group.Dps1 ), 
                ( Role.Dps, group.Dps2 ), 
                ( Role.Dps, group.Dps3 ), 
            }.Where(p => p.Item2 != null).ToList();
        }   
    }
}
