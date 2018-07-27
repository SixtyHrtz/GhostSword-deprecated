using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class ItemType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}