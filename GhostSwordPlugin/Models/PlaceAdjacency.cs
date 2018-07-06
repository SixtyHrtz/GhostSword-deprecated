using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class PlaceAdjacency
    {
        public int Id { get; set; }
        public int Place1Id { get; set; }
        public int Place2Id { get; set; }
        public uint JourneyDuration { get; set; }
        [Required]
        public string BeginText { get; set; }
        [Required]
        public string EndText { get; set; }

        public Place Place1 { get; set; }
        public Place Place2 { get; set; }

        public IEnumerable<Journey> Journeys { get; set; }
    }
}
