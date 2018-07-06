namespace GhostSwordPlugin.Models
{
    public class PlayerNpc
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int NpcLinkId { get; set; }
        public uint Phase { get; set; }

        public Player Player { get; set; }
        public NpcLink NpcLink { get; set; }
    }
}
