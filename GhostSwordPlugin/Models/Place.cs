using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class Place
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        [Required]
        public string Name { get; set; }
        public int PlaceLinkId { get; set; }
        public uint Phase { get; set; }

        public PlaceLink PlaceLink { get; set; }

        [NotMapped]
        public string FullName { get { return $"{(Emoji ?? GhostSword.Emoji.PawPrints)}{Name}"; } }
    }
}
