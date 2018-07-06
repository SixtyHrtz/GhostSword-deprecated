namespace GhostSwordPlugin.Models
{
    public class NpcDialogue
    {
        public int Id { get; set; }
        public int NpcId { get; set; }
        public int DialogueId { get; set; }

        public Npc Npc { get; set; }
        public Dialogue Dialogue { get; set; }
    }
}
