using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
