using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class NPC
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Greetings { get; set; }
        public int PlaceId { get; set; }

        public Place Place { get; set; }

        public List<Dialogue> Dialogues { get; set; }
    }
}
