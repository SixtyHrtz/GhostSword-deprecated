using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Dialogue
    {
        public int Id { get; set; }
        public int NpcInfoId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Text { get; set; }

        public NpcInfo NpcInfo { get; set; }
    }
}
