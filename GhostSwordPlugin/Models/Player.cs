using GhostSword.Interfaces;
using GhostSwordPlugin.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GhostSwordPlugin.Models
{
    public class Player : IUser
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public bool IsBusy { get; set; }
        public int PlaceId { get; set; }
        public MenuType MenuId { get; set; }

        public Guid? HeadItemGuid { get; set; }
        public Guid? ChestItemGuid { get; set; }
        public Guid? HandsItemGuid { get; set; }
        public Guid? LegsItemGuid { get; set; }
        public Guid? FeetsItemGuid { get; set; }

        public Place Place { get; set; }

        public PlayerItem HeadItem { get; set; }
        public PlayerItem ChestItem { get; set; }
        public PlayerItem HandsItem { get; set; }
        public PlayerItem LegsItem { get; set; }
        public PlayerItem FeetsItem { get; set; }

        public IEnumerable<PlayerPlace> PlayerPlaces { get; set; }
        public IEnumerable<PlayerNpc> PlayerNpcs { get; set; }
        public IEnumerable<PlayerItem> PlayerItems { get; set; }
        public IEnumerable<Journey> Journeys { get; set; }

        public Player() { }

        public Player(long id, string username) : this()
        {
            UserId = id;
            Username = username;
        }
    }
}
