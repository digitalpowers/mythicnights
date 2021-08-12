using System.ComponentModel.DataAnnotations;

namespace MythicNights.DataContext
{
    public class Toon
    {
        [ Key ]
        public string FullName { get { return $"{Name}-{Realm}"; } set { } }

        public string Name { get; set; }

        public string Realm { get; set; }

        public double iLvl { get; set; }
        public double RaiderIO { get; set; }
        public Role PreferedRole { get; set; }
        public Role? Offspec { get; set; }

        public bool? IsPrefered { get; set; }

        public override string ToString()
        {
            return $"({Name}:{PreferedRole}:{iLvl})";
        }
    }
}
