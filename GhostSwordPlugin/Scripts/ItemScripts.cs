using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Enums;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GhostSwordPlugin
{
    public partial class GsRepository
    {
        public Message InspectInventory(GsContext context, Player player)
        {
            context.Attach(player);
            player.MenuId = MenuType.Inventory;
            context.SaveChanges();

            string equipment = string.Empty;
            if (player.HeadItem != null) equipment += $"{player.HeadItem.Item.FullName} /rem_head\n";
            if (player.ChestItem != null) equipment += $"{player.ChestItem.Item.FullName} /rem_chest\n";
            if (player.HandsItem != null) equipment += $"{player.HandsItem.Item.FullName} /rem_hands\n";
            if (player.LegsItem != null) equipment += $"{player.LegsItem.Item.FullName} /rem_legs\n";
            if (player.FeetsItem != null) equipment += $"{player.FeetsItem.Item.FullName} /rem_feets\n";
            if (!string.IsNullOrEmpty(equipment)) equipment = $"\n\n{equipment}";

            var items = context.PlayerItems
                .Include(x => x.Item)
                .Where(x => x.PlayerId == player.Id)
                .ToList();

            items.ForEach((x) => x.Amount -= (uint)((
                x.Guid == player.HeadItemGuid ||
                x.Guid == player.ChestItemGuid ||
                x.Guid == player.HandsItemGuid ||
                x.Guid == player.LegsItemGuid ||
                x.Guid == player.FeetsItemGuid) ? 1 : 0));

            var backpack = string.Join("\n", items.Where(x => x.Amount != 0)
                .Select(x => $"{x.Item.FullName} x{x.Amount} /use_{x.ItemId}"));
            backpack = (string.IsNullOrEmpty(backpack)) ? GsResources.BackpackIsEmpty : backpack;
            backpack = (!string.IsNullOrEmpty(equipment)) ? $"\n{backpack}" : $"\n\n{backpack}";

            return new Message($"{Emoji.SchoolBackpack} <b>{GsResources.BackpackContent}:</b>{equipment}{backpack}");
        }

        public Message GetDropItemList(GsContext context, Player player)
        {
            var backpack = string.Join("\n", context.PlayerItems
                .Where(x => x.PlayerId == player.Id)
                .Select(x => $"{x.Item.FullName} x{x.Amount} /drop_{x.ItemId}_1"));
            backpack = (string.IsNullOrEmpty(backpack)) ? GsResources.BackpackIsEmpty : backpack;

            return new Message($"{Emoji.SchoolBackpack} <b>{GsResources.ItemsToDrop}:</b>\n\n{backpack}");
        }

        public Message DropItem(GsContext context, Player player, uint itemTypeId, uint amount)
        {
            var item = context.PlayerItems
                .Include(x => x.Item)
                .Where(x => x.PlayerId == player.Id && x.ItemId == itemTypeId)
                .FirstOrDefault();

            if (item == null)
                return new Message($"{GsResources.BackpackItemNotExists}");

            if (item.Amount < amount)
                return new Message($"{GsResources.BackpackItemsCountOverflow}");

            item.Amount -= amount;
            if (item.Amount == 0)
            {
                if (player.HeadItemGuid == item.Guid)
                    player.HeadItemGuid = null;
                if (player.ChestItemGuid == item.Guid)
                    player.ChestItemGuid = null;
                if (player.HandsItemGuid == item.Guid)
                    player.HandsItemGuid = null;
                if (player.LegsItemGuid == item.Guid)
                    player.LegsItemGuid = null;
                if (player.FeetsItemGuid == item.Guid)
                    player.FeetsItemGuid = null;

                context.Remove(item);
            }

            context.SaveChanges();

            return new Message($"{GsResources.Dropped}: {amount} {item.Item.FullName}");
        }

        public Message UseItem(GsContext context, Player player, uint itemTypeId)
        {
            var item = context.PlayerItems
                .Include(x => x.Item)
                .Where(x => x.PlayerId == player.Id && x.ItemId == itemTypeId)
                .FirstOrDefault();

            if (item == null)
                return new Message($"{GsResources.BackpackItemNotExists}");

            context.Players.Attach(player);
            switch (item.Item.ItemTypeId)
            {
                case 2: player.HeadItemGuid = item.Guid; break;
                case 3: player.ChestItemGuid = item.Guid; break;
                case 4: player.HandsItemGuid = item.Guid; break;
                case 5: player.LegsItemGuid = item.Guid; break;
                case 6: player.FeetsItemGuid = item.Guid; break;
            }
            context.SaveChanges();

            return new Message($"{GsResources.Equiped}: {item.Item.FullName}");
        }

        public Message RemoveItem(GsContext context, Player player, string itemType)
        {
            context.Players.Attach(player);
            Item item;

            switch (itemType)
            {
                case "head":
                    item = player.HeadItem?.Item;
                    player.HeadItemGuid = null;
                    break;
                case "chest":
                    item = player.ChestItem?.Item;
                    player.ChestItemGuid = null;
                    break;
                case "hands":
                    item = player.HandsItem?.Item;
                    player.HandsItemGuid = null;
                    break;
                case "legs":
                    item = player.LegsItem?.Item;
                    player.LegsItemGuid = null;
                    break;
                case "feets":
                    item = player.FeetsItem?.Item;
                    player.FeetsItemGuid = null;
                    break;
                default:
                    return new Message(GsResources.ItemSlotNotExists);
            }

            context.SaveChanges();

            if (item == null)
                return new Message(GsResources.ItemSlotIsEmpty);
            return new Message($"{GsResources.Removed}: {item.FullName}");
        }
    }
}