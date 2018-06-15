using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace GhostSwordPlugin
{
    public class GsRepository
    {
        public bool Locked { get; private set; }

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
            var player = context.Players.FirstOrDefault(x => x.UserId == message.Id);
            if (player == null)
            {
                var name = message.Username ?? $"{message.FirstName} {message.LastName}";
                context.Players.Add(player = new Player(message.Id, name));
                context.SaveChanges();

                context.Database.ExecuteSqlCommand("EXEC InitializePlayerPlaces @PlayerId",
                    new SqlParameter("PlayerId", player.Id));
            }

            return player;
        }

        public Data<Keyboard> GetKeyboard(GsContext context, IUser user)
        {
            Button[] buttons;
            switch (((Player)user).MenuId)
            {
                case 1:
                    buttons = new Button[]
                    {
                        GsResources.LookAround,
                        GsResources.Backpack
                    };
                    break;

                case 2:
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
            var places = GetAdjacentPlaces(context, player).Text;
            var npcs = GetNPCs(context, player).Text;
            return new Message($"{Emoji.Eye} <b>{GsResources.Nearby}:</b>\n\n{places}\n{npcs}");
        }

        public Message BackToPrevMenu(GsContext context, Player player)
        {
            context.Attach(player);

            switch (player.MenuId)
            {
                case 2: player.MenuId = 1; break;
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
                return new Message($"{GsResources.PlaceNotExists}!");

            var placeAdjacency = context.PlaceAdjacencies
                .Where(x => x.Place1Id == player.PlaceId && x.Place2Id == place.foundPlace.Id)
                .FirstOrDefault();

            if (placeAdjacency == null)
                return new Message($"{GsResources.PlaceTooFar}!");

            context.Attach(player);
            player.IsBusy = true;
            context.Journeys.Add(new Journey(player, placeAdjacency, DateTime.Now));
            context.SaveChanges();

            return new Message(placeAdjacency?.BeginText);
        }

        public Message GetNPCs(GsContext context, Player player) =>
            new Message(string.Join("\n", context.NPCs
                .Where(x => x.PlaceId == player.PlaceId)
                .Select(x => $"{Emoji.BustInSilhouette}{x.Name} /npc_{x.Id}")));

        public Message GetDialogues(GsContext context, Player player, uint npcId)
        {
            if (player.IsBusy)
                return GsResources.PlayerIsBusy;

            var npc = context.NPCs
                .Include(x => x.Dialogues)
                .FirstOrDefault(x => x.Id == npcId);

            if (npc == null)
                return GetLookupMessage(context, player, $"{GsResources.NPCNotExists}!");

            if (npc.PlaceId != player.PlaceId)
                return GetLookupMessage(context, player, $"{GsResources.NPCTooFar}!");

            if (npc.Dialogues.Count == 0)
                return GetLookupMessage(context, player, GsResources.NothingToTalkAbout);

            return new Message($"{npc.Greetings}\n\n" + string.Join("\n", npc.Dialogues
                .Select(x => $"{Emoji.SpeechBalloon}{x.Name} /dial_{x.Id}")));
        }

        public Message GetDialogue(GsContext context, Player player, uint dialogueId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var dialogue = context.Dialogues
                .Include(x => x.NPC)
                .FirstOrDefault(x => x.Id == dialogueId);

            if (dialogue == null)
                return GetLookupMessage(context, player, $"{GsResources.DialogNotExists}!");

            if (dialogue.NPC.PlaceId != player.PlaceId)
                return GetLookupMessage(context, player, $"{GsResources.NPCTooFar}!");

            return new Message($"<b>{dialogue.Name}</b>\n{dialogue.Text}");
        }

        public Message InspectBackpack(GsContext context, Player player)
        {
            context.Attach(player);
            player.MenuId = 2;
            context.SaveChanges();

            var backpack = string.Join("\n", context.PlayerItems
                .Where(x => x.PlayerId == player.Id)
                .Select(x => $"{x.Item.Emoji} {x.Item.Name} x{x.Amount}"));
            backpack = (string.IsNullOrEmpty(backpack)) ? GsResources.BackpackIsEmpty : backpack;

            return new Message($"{Emoji.SchoolBackpack} <b>{GsResources.BackpackContent}:</b>\n\n{backpack}");
        }

        public Message GetDropItemList(GsContext context, Player player)
        {
            var backpack = string.Join("\n", context.PlayerItems
                .Where(x => x.PlayerId == player.Id)
                .Select(x => $"{x.Item.Emoji} {x.Item.Name} x{x.Amount} /drop_{x.ItemId}_1"));
            backpack = (string.IsNullOrEmpty(backpack)) ? GsResources.BackpackIsEmpty : backpack;

            return new Message($"{Emoji.SchoolBackpack} <b>{GsResources.ItemsToDrop}:</b>\n\n{backpack}");
        }

        public Message DropItem(GsContext context, Player player, uint itemId, uint amount)
        {
            var item = context.PlayerItems
                .Include(x => x.Item)
                .Where(x => x.PlayerId == player.Id && x.ItemId == itemId)
                .FirstOrDefault();

            if (item == null)
                return new Message($"{GsResources.ItemIsNotInBackpack}!\n\n{GetDropItemList(context, player)}");

            if (item.Amount < amount)
                return new Message($"{GsResources.ThereIsNoSoManyItemsInBackPack}!\n\n{GetDropItemList(context, player)}");

            item.Amount -= amount;
            context.SaveChanges();

            return new Message($"{GsResources.Dropped}: {amount} {item.Item.Emoji} {item.Item.Name}!\n\n{GetDropItemList(context, player)}");
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
