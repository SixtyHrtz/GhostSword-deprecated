using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class NpcInfo
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Greetings { get; set; }
    }
}
