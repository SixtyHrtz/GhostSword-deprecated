using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class Class
    {
        public int Id { get; set; }
        [Required]
        public string Emoji { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ExperienceEmoji { get; set; }
        [Required]
        public string ExperienceName { get; set; }
        [Required]
        public string EnergyEmoji { get; set; }
        [Required]
        public string EnergyName { get; set; }
        [Required]
        public string IdleAction { get; set; }

        [NotMapped]
        public string FullExperience { get { return ExperienceEmoji + ExperienceName; } }
        [NotMapped]
        public string FullEnergy { get { return EnergyEmoji + EnergyName; } }
    }
}
