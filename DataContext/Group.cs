namespace MythicNights.DataContext
{
    public class Group
    {
        public Player Tank { get; set; }
        public Player Healer { get; set; }
        public Player Dps1 { get; set; }
        public Player Dps2 { get; set; }
        public Player Dps3 { get; set; }

        public override string ToString()
        {
            return $"{Tank}/{Healer}/{Dps1}/{Dps2}/{Dps3}";
        }
    }
}
