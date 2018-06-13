using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhostSwordPlugin.Models
{
    public class PlayerItem
    {
        [Key]
        public Guid Guid { get; set; }
        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public uint Amount { get; set; }

        public Player Player { get; set; }
        public Item Item { get; set; }

        [NotMapped]
        public ItemDiscovery ItemDiscovery { get; set; }

        public PlayerItem() { }

        public PlayerItem(Player player, Item item, uint amount, ItemDiscovery itemDiscovery)
            : this(player, item, amount) =>
            ItemDiscovery = itemDiscovery;

        public PlayerItem(Player player, Item item, uint amount)
        {
            Player = player;
            PlayerId = player.Id;
            Item = item;
            ItemId = item.Id;
            Amount = amount;
        }
    }
}
