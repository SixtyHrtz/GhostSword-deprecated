using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class PlaceLink
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
