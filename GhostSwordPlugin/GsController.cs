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
    public partial class GsController
    {
        public bool Locked { get; private set; }

        private readonly List<Func<GsContext, List<AnswerMessage>>> eventMethods;

        public GsController()
        {
            using (var context = new GsContext())
                context.Database.ExecuteSqlCommand("EXEC InitializePlayerPhases");

            eventMethods = new List<Func<GsContext, List<AnswerMessage>>>
            {
                ExtractJournes,
                UpdateStamina
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
                .Include(p => p.Class)
                .Include(p => p.HeadItem).ThenInclude(pi => pi.Item)
                .Include(p => p.ChestItem).ThenInclude(pi => pi.Item)
                .Include(p => p.HandsItem).ThenInclude(pi => pi.Item)
                .Include(p => p.LegsItem).ThenInclude(pi => pi.Item)
                .Include(p => p.FeetsItem).ThenInclude(pi => pi.Item)
                .Include(p => p.LeftHandItem).ThenInclude(pi => pi.Item)
                .Include(p => p.RightHandItem).ThenInclude(pi => pi.Item)
                .FirstOrDefault(p => p.UserId == message.Id);

            if (player == null)
            {
                var name = message.Username ?? $"{message.FirstName} {message.LastName}";
                context.Players.Add(player = new Player(message.Id, name));
                context.SaveChanges();

                context.Database.ExecuteSqlCommand("EXEC InitializePlayerPhases");
            }

            return player;
        }

        public Data<Keyboard> GetKeyboard(GsContext context, IUser user)
        {
            Button[][] buttons;
            switch (((Player)user).MenuId)
            {
                case MenuType.Main:
                    buttons = new Button[][]
                    {
                        new Button[]
                        {
                            GsResources.LookAround
                        },
                        new Button[]
                        {
                            GsResources.Profile,
                            GsResources.Backpack
                        }
                    };
                    break;

                case MenuType.Inventory:
                    buttons = new Button[][]
                    {
                        new Button[]
                        {
                            GsResources.Backpack,
                            GsResources.Drop,
                            GsResources.Back
                        }
                    };
                    break;

                default:
                    buttons = null;
                    break;
            }

            return Data<Keyboard>.CreateValid(new Keyboard(buttons));
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

        private Message GetLookupMessage(GsContext context, Player player, string text)
        {
            var lookup = LookAround(context, player).Text;
            return new Message($"{text}\n\n{lookup}");
        }

        public Message LookAround(GsContext context, Player player)
        {
            var dayTime = GetTimeOfDay();
            var places = GetAdjacentPlaces(context, player).Text;
            var npcs = GetNpcs(context, player).Text;
            return new Message($"{dayTime}\n\n{Emoji.Eye} <b>{GsResources.Nearby}:</b>\n{places}\n{npcs}");
        }

        private readonly string[] weatherEmojies = new string[]
        {
            Emoji.NewMoonFace,
            Emoji.CrescentMoon,
            Emoji.SunWithFace,
            Emoji.CrescentMoon
        };
        private readonly string[] weatherNames = new string[] { "Ночь", "Утро", "День", "Вечер" };

        private string GetTimeOfDay()
        {
            var coefficient = 8 / 24f;

            var newSeconds = DateTime.Now.TimeOfDay.TotalSeconds / coefficient;
            newSeconds = (newSeconds % 28800) * 3;

            var newTime = TimeSpan.FromSeconds(newSeconds);
            newTime = TimeSpan.FromHours(18);
            var fourthIndex = (int)Math.Floor(newTime.Hours / 6.0);

            var emojiIndex = fourthIndex;
            var nameIndex = fourthIndex;

            return $"<b>{GsResources.CurrentDayTime}:</b> {weatherEmojies[emojiIndex]} {weatherNames[nameIndex]}";
        }

        public Message GetProfile(GsContext context, Player player)
        {
            var profile = $"{player.Class.Emoji}{player.Username}, {player.Level}lvl\n" +
                $"<i>«{player.Class.IdleAction}»</i>\n" +
                $"{player.Class.FullExperience}: {player.Experience}/{player.ExperienceToNextLevel}\n" +
                $"{GsResources.Health}: {player.Health}/{player.TotalHealth}\n" +
                $"{GsResources.Stamina}: {player.Stamina}/{player.TotalStamina} ({Emoji.HourglassNotDone}<i>{player.GetStaminaRecoveryTime()}</i> мин)\n" +
                $"{player.Class.FullEnergy}: {player.Energy}/{player.TotalEnergy}\n\n" +
                $"{GsResources.Characteristics}:\n" +
                $"{GsResources.MeleeAttack}: {player.TotalMeleeAttack}\n" +
                $"{GsResources.MeleeDefence}: {player.TotalPhysicDefence}\n\n" +
                $"{GsResources.Equipment}: /eqp\n" +
                $"{GsResources.Journal}: /jrn\n" +
                $"{GsResources.Bag}: /bag";

            return new Message(profile);
        }

        public Message GetAdjacentPlaces(GsContext context, Player player) =>
            new Message(string.Join("\n", context.PlaceAdjacencies
                .Include(pa => pa.Place2)
                    .ThenInclude(p => p.PlaceLink)
                .Where(pa => pa.Place1Id == player.PlaceId && context.PlayerPlaces
                    .Where(pp => pp.PlayerId == player.Id)
                    .Any(pp => pp.PlaceLinkId == pa.Place2.PlaceLinkId && pp.Phase == pa.Place2.Phase))
                .Select(pa => $"{Emoji.WhiteQuestionMark}{pa.Place2.Name} /place_{pa.Place2.PlaceLink.Name}")));

        public Message BeginJourney(GsContext context, Player player, string placeLinkName)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var place = context.PlayerPlaces
                .Include(pp => pp.PlaceLink)
                .Join(context.Places,
                    x => new { x.PlaceLinkId, x.Phase },
                    y => new { y.PlaceLinkId, y.Phase },
                    (playerPlace, foundPlace) => new { playerPlace, foundPlace })
                .FirstOrDefault(x => x.playerPlace.PlaceLink.Name == placeLinkName &&
                    x.playerPlace.PlayerId == player.Id)?.foundPlace;

            if (place == null)
                return new Message(GsResources.PlaceNotExists);

            var placeAdjacency = context.PlaceAdjacencies
                .Where(pa => pa.Place1Id == player.PlaceId && pa.Place2Id == place.Id)
                .FirstOrDefault();

            if (placeAdjacency == null)
                return new Message(GsResources.PlaceTooFar);

            context.Attach(player);
            player.IsBusy = true;
            context.Journeys.Add(new Journey(player, placeAdjacency, DateTime.Now));
            context.SaveChanges();

            return new Message(placeAdjacency?.BeginText);
        }

        public List<AnswerMessage> ExtractJournes(GsContext context)
        {
            var result = new List<AnswerMessage>();
            var now = DateTime.Now;

            var journeyList = context.Journeys
                .Include(j => j.Player)
                .Include(j => j.PlaceAdjacency)
                .Where(j => (j.StartTime.AddSeconds(j.Duration) < now))
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
                .Include(id => id.Item)
                .Where(id => (id.PlaceId == placeAdjacency.Place1Id) || (id.PlaceId == placeAdjacency.Place2Id))
                .ToList()
                .Where(id => id.Rate >= GhostSword.Random.Percent())
                .Select(id => new PlayerItem(player, id.Item, GhostSword.Random.UnsignedInteger(id.MinAmount, id.MaxAmount), id))
                .ToList();

            var message = string.Join(' ', playerItems
                 .Select(pi => pi.ItemDiscovery.Text
                     .Replace("[VALUE]", $"{pi.Item.Emoji} {pi.Amount} {pi.Item.Name.ToLower()}")));

            context.PlayerItems
                .Join(playerItems,
                    x => new { x.PlayerId, x.ItemId },
                    y => new { y.PlayerId, y.ItemId },
                    (source, destination) => new { source, destination })
                .ToList()
                .ForEach((x) => x.source.Amount += x.destination.Amount);

            context.PlayerItems.AddRange(playerItems
                .Where(pi => !context.PlayerItems
                    .Any(pi1 => pi1.PlayerId == pi.PlayerId && pi1.ItemId == pi.ItemId))
                .ToList());

            context.SaveChanges();
            return message;
        }

        public List<AnswerMessage> UpdateStamina(GsContext context)
        {
            context.Players
                .Where(p => p.Stamina < p.TotalStamina && p.StartRecoveryTime == null)
                .ToList()
                .ForEach(p => p.StartRecoveryTime = DateTime.Now);

            context.Players
                .Where(p => p.Stamina < p.TotalStamina && p.StartRecoveryTime != null &&
                    (DateTime.Now - p.StartRecoveryTime).Value.TotalMinutes >= 30)
                .ToList()
                .ForEach(p =>
                {
                    p.StartRecoveryTime = DateTime.Now;
                    p.Stamina++;
                });

            context.Players
                .Where(p => p.Stamina >= p.TotalStamina && p.StartRecoveryTime != null)
                .ToList()
                .ForEach(p => p.StartRecoveryTime = null);

            context.SaveChanges();
            return new List<AnswerMessage>();
        }
    }
}
