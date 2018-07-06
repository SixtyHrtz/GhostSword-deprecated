using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Dialogue
    {
        public int Id { get; set; }
        [Required]
        public string LinkName { get; set; }
        [Required]
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
