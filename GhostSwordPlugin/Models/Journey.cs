using System;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Journey
    {
        [Key]
        public Guid Guid { get; set; }
        public int PlayerId { get; set; }
        public int PlaceAdjacencyId { get; set; }
        public int Duration { get; set; }
        [Required]
        public DateTime StartTime { get; set; }

        public Player Player { get; set; }
        public PlaceAdjacency PlaceAdjacency { get; set; }

        public Journey() { }

        public Journey(Player player, PlaceAdjacency placeAdjacency, DateTime startTime)
        {
            Player = player;
            PlayerId = player.Id;
            PlaceAdjacency = placeAdjacency;
            PlaceAdjacencyId = placeAdjacency.Id;
            Duration = placeAdjacency.JourneyDuration;

            StartTime = startTime;
        }
    }
}
