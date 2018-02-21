using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Place
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
