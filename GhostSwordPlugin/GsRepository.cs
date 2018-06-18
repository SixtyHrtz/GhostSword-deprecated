using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordPlugin.Enums;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostSwordPlugin
{
    public class GsRepository
    {
        public bool Locked { get; private set; }

        private readonly string[] weatherEmojies = new string[] { "🌙", "☀️" };

        private readonly List<Func<GsContext, List<AnswerMessage>>> eventMethods;

        public GsRepository()
        {
            using (var context = new GsContext())
                context.Database.ExecuteSqlCommand("EXEC InitializePlayersPlaces");

            eventMethods = new List<Func<GsContext, List<AnswerMessage>>>
            {
                ExtractJournes
            };
        }

        public void Lock() => Locked = true;
        public void Unlock() => Locked = false;

        public List<AnswerMessage> GetEventsResults(GsContext context)
        {
            var messages = new List<AnswerMessage>();
            foreach (var method in eventMethods)
                messages.AddRange(method(context));

            return messages;
        }

        public Data<IUser> GetUser(GsContext context, IncomeMessage message) =>
            Data<IUser>.CreateValid(GetOrInsertPlayer(context, message));

        public Player GetOrInsertPlayer(GsContext context, IncomeMessage message)
        {
            var player = context.Players
                .Include(x => x.HeadItem).ThenInclude(y => y.Item)
                .Include(x => x.ChestItem).ThenInclude(y => y.Item)
                .Include(x => x.HandsItem).ThenInclude(y => y.Item)
                .Include(x => x.LegsItem).ThenInclude(y => y.Item)
                .Include(x => x.FeetsItem).ThenInclude(y => y.Item)
                .FirstOrDefault(x => x.UserId == message.Id);

            if (player == null)
            {
                var name = message.Username ?? $"{message.FirstName} {message.LastName}";
                context.Players.Add(player = new Player(message.Id, name));
                context.SaveChanges();

                context.PlayerPlaces.AddRange(
                    context.PlaceLinks
                        .Select(x => new PlayerPlace()
                        {
                            PlayerId = player.Id,
                            PlaceLinkId = x.Id
                        }));
                context.SaveChanges();
            }

            return player;
        }

        public Data<Keyboard> GetKeyboard(GsContext context, IUser user)
        {
            Button[] buttons;
            switch (((Player)user).MenuId)
            {
                case MenuType.Main:
                    buttons = new Button[]
                    {
                        GsResources.LookAround,
                        GsResources.Backpack
                    };
                    break;

                case MenuType.Inventory:
                    buttons = new Button[]
                    {
                        GsResources.Backpack,
                        GsResources.Drop,
                        GsResources.Back
                    };
                    break;

                default:
                    buttons = null;
                    break;
            }

            return Data<Keyboard>.CreateValid(new Keyboard(buttons));
        }

        private Message GetLookupMessage(GsContext context, Player player, string text)
        {
            var lookup = LookAround(context, player).Text;
            return new Message($"{text}\n\n{lookup}");
        }

        public Message LookAround(GsContext context, Player player)
        {
            var dayTime = GetTimeOfDay();
            var places = GetAdjacentPlaces(context, player).Text;
            var npcs = GetNPCs(context, player).Text;
            return new Message($"{dayTime}\n\n{Emoji.Eye} <b>{GsResources.Nearby}:</b>\n{places}\n{npcs}");
        }

        private string GetTimeOfDay()
        {
            var coefficient = 8 / 24f;

            var newSeconds = DateTime.Now.TimeOfDay.TotalSeconds / coefficient;
            newSeconds = (newSeconds % 28800) * 3;

            var newTime = TimeSpan.FromSeconds(newSeconds);
            var emojiIndex = (newTime.Hours < 8 || newTime.Hours >= 20) ? 0 : 1;

            return $"<b>{GsResources.CurrentDayTime}:</b> {weatherEmojies[emojiIndex]}";
        }

        public Message BackToPrevMenu(GsContext context, Player player)
        {
            context.Attach(player);

            switch (player.MenuId)
            {
                case MenuType.Inventory: player.MenuId = MenuType.Main; break;
            }

            context.SaveChanges();
            return LookAround(context, player);
        }

        public Message GetAdjacentPlaces(GsContext context, Player player) =>
            new Message(string.Join("\n", context.PlaceAdjacencies
                .Include(x => x.Place2)
                    .ThenInclude(y => y.PlaceLink)
                .Where(x => x.Place1Id == player.PlaceId && context.PlayerPlaces
                    .Where(y => y.PlayerId == player.Id)
                    .Any(y => y.PlaceLinkId == x.Place2.PlaceLinkId && y.Phase == x.Place2.Phase))
                .Select(x => $"{Emoji.WhiteQuestionMark}{x.Place2.Name} /place_{x.Place2.PlaceLink.Name}")));

        public Message BeginJourney(GsContext context, Player player, string placeLinkName)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var place = context.PlayerPlaces
                .Include(x => x.PlaceLink)
                .Join(context.Places,
                    x => new { x.PlaceLinkId, x.Phase },
                    y => new { y.PlaceLinkId, y.Phase },
                    (playerPlace, foundPlace) => new { playerPlace, foundPlace })
                .FirstOrDefault(x => x.playerPlace.PlaceLink.Name == placeLinkName &&
                    x.playerPlace.PlayerId == player.Id);

            if (place == null)
                return new Message(GsResources.PlaceNotExists);

            var placeAdjacency = context.PlaceAdjacencies
                .Where(x => x.Place1Id == player.PlaceId && x.Place2Id == place.foundPlace.Id)
                .FirstOrDefault();

            if (placeAdjacency == null)
                return new Message(GsResources.PlaceTooFar);

            context.Attach(player);
            player.IsBusy = true;
            context.Journeys.Add(new Journey(player, placeAdjacency, DateTime.Now));
            context.SaveChanges();

            return new Message(placeAdjacency?.BeginText);
        }

        public Message GetNPCs(GsContext context, Player player) =>
            new Message(string.Join("\n", context.NpcInfos
                .Where(x => x.PlaceId == player.PlaceId)
                .Select(x => $"{Emoji.BustInSilhouette}{x.Name} /npc_{x.Id}")));

        public Message GetDialogues(GsContext context, Player player, uint npcId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var npc = context.NpcInfos
                .Include(x => x.Dialogues)
                .FirstOrDefault(x => x.Id == npcId);

            if (npc == null)
                return GetLookupMessage(context, player, GsResources.NpcNotExists);

            if (npc.PlaceId != player.PlaceId)
                return GetLookupMessage(context, player, GsResources.NpcTooFar);

            if (npc.Dialogues.Count == 0)
                return GetLookupMessage(context, player, GsResources.NothingToTalkAbout);

            return new Message($"{npc.Greetings}\n\n" + string.Join("\n", npc.Dialogues
                .Select(x => $"{Emoji.SpeechBalloon}{x.Name} /dial_{x.Id}")));
        }

        public Message GetDialogue(GsContext context, Player player, uint dialogueId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var dialogue = context.Dialogues
                .Include(x => x.NpcInfo)
                .FirstOrDefault(x => x.Id == dialogueId);

            if (dialogue == null)
                return GetLookupMessage(context, player, GsResources.DialogNotExists);

            if (dialogue.NpcInfo.PlaceId != player.PlaceId)
                return GetLookupMessage(context, player, GsResources.NpcTooFar);

            return new Message($"<b>{dialogue.Name}</b>\n{dialogue.Text}");
        }

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
            player.ChestItemGuid = item.Guid;
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

        public List<AnswerMessage> ExtractJournes(GsContext context)
        {
            var result = new List<AnswerMessage>();
            var now = DateTime.Now;

            var journeyList = context.Journeys
                .Include(x => x.Player)
                .Include(x => x.PlaceAdjacency)
                .Where(x => (x.StartTime.AddSeconds(x.Duration) < now))
                .ToList();

            foreach (Journey journey in journeyList)
            {
                var player = journey.Player;

                context.Attach(player);
                player.IsBusy = false;
                player.PlaceId = journey.PlaceAdjacency.Place2Id;

                var lookup = LookAround(context, journey.Player).Text;

                Message message;
                var drop = ProcessDiscoveryItems(context, player, journey.PlaceAdjacency);
                if (drop == string.Empty)
                    message = new Message($"{journey.PlaceAdjacency?.EndText}\n\n{lookup}");
                else
                    message = new Message($"{journey.PlaceAdjacency?.EndText}\n\n{drop}\n\n{lookup}");

                result.Add(new AnswerMessage(player, message));
            }

            context.Journeys.RemoveRange(journeyList);
            context.SaveChanges();
            return result;
        }

        private string ProcessDiscoveryItems(GsContext context, Player player, PlaceAdjacency placeAdjacency)
        {
            context.Players.Attach(player);

            var playerItems = context.ItemDiscoveries
                .Include(x => x.Item)
                .Where(x =>
                    (x.PlaceId == placeAdjacency.Place1Id) ||
                    (x.PlaceId == placeAdjacency.Place2Id))
                .ToList().Where(x => x.Rate >= GhostSword.Random.Percent())
                .Select(x => new PlayerItem(player, x.Item, GhostSword.Random.UnsignedInteger(x.MinAmount, x.MaxAmount), x))
                .ToList();

            var message = string.Join(' ', playerItems
                 .Select(x => x.ItemDiscovery.Text
                     .Replace("[VALUE]", $"{x.Item.Emoji} {x.Amount} {x.Item.Name.ToLower()}")));

            context.PlayerItems
                .Join(playerItems,
                    x => new { x.PlayerId, x.ItemId },
                    y => new { y.PlayerId, y.ItemId },
                    (source, destination) => new { source, destination })
                .ToList()
                .ForEach((x) => x.source.Amount += x.destination.Amount);

            context.PlayerItems.AddRange(playerItems.Where(x => !context.PlayerItems
                .Any(y => y.PlayerId == x.PlayerId && y.ItemId == x.ItemId))
                .ToList());

            context.SaveChanges();
            return message;
        }
    }
}
