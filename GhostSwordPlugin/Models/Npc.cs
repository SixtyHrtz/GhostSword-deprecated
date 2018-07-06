using System.Collections.Generic;

namespace GhostSwordPlugin.Models
{
    public class Npc
    {
        public int Id { get; set; }
        public uint Phase { get; set; }
        public int NpcLinkId { get; set; }
        public int NpcInfoId { get; set; }
        public int PlaceId { get; set; }

        public NpcLink NpcLink { get; set; }
        public NpcInfo NpcInfo { get; set; }
        public Place Place { get; set; }

        public IEnumerable<NpcDialogue> NpcDialogues { get; set; }
    }
}
