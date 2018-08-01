using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class NpcInfo
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Greetings { get; set; }

        [NotMapped]
        public string FullName { get { return $"{(Emoji ?? GhostSword.Emoji.BustInSilhouette)}{Name}"; } }
    }
}
