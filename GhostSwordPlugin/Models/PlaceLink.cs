﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class PlaceLink
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public IEnumerable<PlayerPlace> PlayerPlaces { get; set; }
        public IEnumerable<Place> Places { get; set; }
    }
}
