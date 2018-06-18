using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public string FullName { get { return $"{Emoji} {Name}"; } }
    }
}
