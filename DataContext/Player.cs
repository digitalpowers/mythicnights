using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MythicNights.DataContext
{
    public class Player
    {
        [Key]
        public ulong DiscordUserId { get; set; }
        public string DiscordUsername { get; set; }
        public List<Toon> Toons { get; set; } = new List<Toon>();
        public string Nickname { get; set; }

        public override string ToString()
        {
            return Nickname;
        }
        public bool PrefersRole(Role role)
        {
            return Toons.Any(t => t.PreferedRole == role);
        }
        public bool CanDoRole(Role role)
        {
            return Toons.Any(t => t.PreferedRole == role || t.Offspec == role);
        }
    }
}
