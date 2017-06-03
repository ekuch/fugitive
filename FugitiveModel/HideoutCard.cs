namespace FugitiveModel
{
    public class HideoutCard
    {
        public HideoutCard(int hideout)
        {
            Hideout = hideout;
            Steps = 2 - (hideout & 1);
        }

        public int Hideout { get; set; }
        public int Steps { get; set; }

        public static HideoutCard InvisibleCard = new HideoutCard(0);
    }
}
