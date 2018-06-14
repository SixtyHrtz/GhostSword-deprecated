namespace GhostSwordPlugin.Models
{
    public class PlayerPlace
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int PlaceLinkId { get; set; }
        public int Phase { get; set; }

        public Player Player { get; set; }
        public PlaceLink PlaceLink { get; set; }
    }
}
