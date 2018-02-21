using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Dialogue
    {
        public int Id { get; set; }
        public int NPCId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Text { get; set; }

        public NPC NPC { get; set; }
    }
}
