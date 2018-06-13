using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
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

        private readonly List<Func<GsContext, List<AnswerMessage>>> eventMethods;

        public GsRepository()
        {
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

        public Message GetAdjacentPlaces(GsContext context, Player player)
        {
            var result = string.Empty;
            result += string.Join("\n", context.PlaceAdjacencies
                .Include(x => x.Place2)
                .Where(x => x.Place1Id == player.PlaceId)
                .Select(x => $"{Emoji.WhiteQuestionMark}{x.Place2.Name} /place_{x.Place2.Id}"));

            return new Message(result);
        }

        public Message BeginJourney(GsContext context, Player player, int placeId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var result = string.Empty;
            var place = context.Places
                .FirstOrDefault(x => x.Id == placeId);

            if (place != null)
            {
                var placeAdjacency = context.PlaceAdjacencies
                    .Where(x => x.Place1Id == player.PlaceId && x.Place2Id == placeId)
                    .FirstOrDefault();

                if (placeAdjacency != null)
                {
                    context.Attach(player);
                    player.IsBusy = true;
                    context.Journeys.Add(new Journey(player, placeAdjacency, System.DateTime.Now));

                    context.SaveChanges();
                    result = placeAdjacency?.BeginText;
                }
                else
                    result += $"{GsResources.PlaceTooFar}!";
            }
            else
                result += $"{GsResources.PlaceNotExists}!";

            return new Message(result);
        }

        public Message GetNPCs(GsContext context, Player player)
        {
            var result = string.Join("\n", context.NPCs
                .Where(x => x.PlaceId == player.PlaceId)
                .Select(x => $"{Emoji.BustInSilhouette}{x.Name} /npc_{x.Id}"));

            return new Message(result);
        }

        public Message GetDialogues(GsContext context, Player player, int npcId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var npc = context.NPCs
                .Include(x => x.Dialogues)
                .FirstOrDefault(x => x.Id == npcId);

            var result = string.Empty;
            if (npc != null)
            {
                if (npc.PlaceId == player.PlaceId)
                {
                    if (npc.Dialogues.Count != 0)
                    {
                        result += $"{npc.Greetings}\n\n";
                        result += string.Join("\n", npc.Dialogues
                            .Select(x => $"{Emoji.SpeechBalloon}{x.Name} /dial_{x.Id}"));
                        return new Message(result);
                    }
                    else
                        result = GsResources.NothingToTalkAbout;
                }
                else
                    result = $"{GsResources.NPCTooFar}!";
            }
            else
                result = $"{GsResources.NPCNotExists}!";

            var lookup = LookAround(context, player).Text;
            return new Message($"{result}\n\n{lookup}");
        }

        public Message GetDialogue(GsContext context, Player player, int dialogueId)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var result = string.Empty;
            var dialogue = context.Dialogues
                .Include(x => x.NPC)
                .FirstOrDefault(x => x.Id == dialogueId);

            if (dialogue != null)
            {
                if (dialogue.NPC.PlaceId == player.PlaceId)
                {
                    result = $"<b>{dialogue.Name}</b>\n{dialogue.Text}";
                    return new Message(result);
                }
                else
                    result = $"{GsResources.NPCTooFar}!";
            }
            else
                result = $"{GsResources.DialogNotExists}!";

            var lookup = LookAround(context, player).Text;
            return new Message($"{result}\n{lookup}");
        }

        public Message InspectBackpack(GsContext context, Player player)
        {
            context.Attach(player);
            player.MenuId = 2;
            context.SaveChanges();

            var backpack = string.Join("\n", context.PlayerItems
                .Where(x => x.PlayerId == player.Id)
                .Select(x => $"{x.Item.Emoji} {x.Item.Name} x{x.Amount}"));

            var result = $"{Emoji.SchoolBackpack} <b>{GsResources.BackpackContent}:</b>\n\n";
            if (string.IsNullOrEmpty(backpack))
                return new Message(result + GsResources.BackpackIsEmpty);
            else
                return new Message(result + backpack);
        }

        public Message GetDropItemList(GsContext context, Player player)
        {
            var backpack = string.Join("\n", context.PlayerItems
                .Where(x => x.PlayerId == player.Id)
                .Select(x => $"{x.Item.Emoji} {x.Item.Name} x{x.Amount} /drop_{x.ItemId}_1"));

            var result = $"{Emoji.SchoolBackpack} <b>{GsResources.ItemsToDrop}:</b>\n\n";
            if (string.IsNullOrEmpty(backpack))
                return new Message($"{result} {GsResources.BackpackIsEmpty}");
            else
                return new Message($"{result} {backpack}");
        }

        public Message DropItem(GsContext context, Player player, int itemId, int amount)
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
            List<AnswerMessage> result = new List<AnswerMessage>();
            var now = DateTime.Now;

            List<Journey> journeyList = context.Journeys
                .Include(x => x.Player)
                .Include(x => x.PlaceAdjacency)
                .Where(x => (x.StartTime.AddSeconds(x.Duration) < now))
                .ToList();

            foreach (Journey journey in journeyList)
            {
                Player player = journey.Player;

                context.Attach(player);
                player.IsBusy = false;
                player.PlaceId = journey.PlaceAdjacency.Place2Id;

                string lookup = LookAround(context, journey.Player).Text;

                Message message;
                string drop = ProcessDiscoveryItems(context, player, journey.PlaceAdjacency);
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
            var message = string.Empty;

            context.Players.Attach(player);

            var playerItems = context.ItemDiscoveries
                .Include(x => x.Item)
                .Where(x =>
                    (x.PlaceId == placeAdjacency.Place1Id) ||
                    (x.PlaceId == placeAdjacency.Place2Id))
                .ToList().Where(x => x.Rate >= GhostSword.Random.Percent())
                .Select(x => new PlayerItem(player, x.Item, GhostSword.Random.Integer(x.MinAmount, x.MaxAmount), x)).ToList();

            message += string.Join(' ', playerItems
                .Select(x => x.ItemDiscovery.Text
                    .Replace("[[Value]]", $"{x.Item.Emoji} {x.Amount} {x.Item.Name.ToLower()}")));

            context.PlayerItems
                .Join(playerItems,
                    x => new { x.PlayerId, x.ItemId },
                    y => new { y.PlayerId, y.ItemId },
                    (source, destination) => new { source, destination })
                .ToList().ForEach((x) => x.source.Amount += x.destination.Amount);

            context.PlayerItems.AddRange(playerItems.Where(x => !context.PlayerItems
                .Any(y => y.PlayerId == x.PlayerId && y.ItemId == x.ItemId)).ToList());

            context.SaveChanges();
            return message;
        }
    }
}
