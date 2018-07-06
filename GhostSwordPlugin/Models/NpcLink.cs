using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class NpcLink
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public IEnumerable<PlayerNpc> PlayerNpcs { get; set; }
        public IEnumerable<Npc> Npcs { get; set; }
    }
}
