﻿using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class ItemDiscovery
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int PlaceId { get; set; }
        [Required]
        public string Text { get; set; }
        public float Rate { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }

        public Item Item { get; set; }
        public Place Place { get; set; }
    }
}
