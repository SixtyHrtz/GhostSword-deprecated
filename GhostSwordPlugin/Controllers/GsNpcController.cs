using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace GhostSwordPlugin
{
    public partial class GsController
    {
        public Message GetNpcs(GsContext context, Player player) =>
            new Message(string.Join("\n", GetAvailableNpcs(context, player)
                .Select(n => $"{Emoji.BustInSilhouette}{n.NpcInfo.Name} /npc_{n.NpcLink.Name}")));

        public Message GetDialogues(GsContext context, Player player, string link)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var npc = GetNpcByLink(context, player, link);

            if (npc == null)
                return GetLookupMessage(context, player, GsResources.NpcNotExists);

            if (npc.PlaceId != player.PlaceId)
                return GetLookupMessage(context, player, GsResources.NpcTooFar);

            if (npc.NpcDialogues.Count() == 0)
                return GetLookupMessage(context, player, GsResources.NothingToTalkAbout);

            return new Message($"{npc.NpcInfo.Greetings}\n\n{GetDialoguesList(npc)}");
        }

        public Message GetDialogue(GsContext context, Player player, string link)
        {
            if (player.IsBusy) return GsResources.PlayerIsBusy;

            var dialogue = GetDialogueByLink(context, player, link);

            if (dialogue == null)
                return GetLookupMessage(context, player, GsResources.DialogNotExists);

            var npc = GetNpcByDialogue(context, player, dialogue);

            if (npc.Count() == 0)
                return GetLookupMessage(context, player, GsResources.DialogUnavailable);

            if (npc.Count(x => x.PlaceId == player.PlaceId) == 0)
                return GetLookupMessage(context, player, GsResources.NpcTooFar);

            return new Message($"<b>{dialogue.Name}</b>\n{dialogue.Text}");
        }

        private string GetDialoguesList(Npc npc) =>
            string.Join("\n", npc.NpcDialogues
                .Select(nd => $"{Emoji.SpeechBalloon}{nd.Dialogue.Name} /dial_{nd.Dialogue.LinkName}"));

        private IEnumerable<Npc> GetAvailableNpcs(GsContext context, Player player) =>
            context.Npcs
                .Include(n => n.NpcLink)
                .Include(n => n.NpcInfo)
                .Where(n => n.PlaceId == player.PlaceId && context.PlayerNpcs
                    .Any(pn => pn.PlayerId == player.Id && pn.NpcLinkId == n.NpcLinkId && pn.Phase == n.Phase));

        private Npc GetNpcByLink(GsContext context, Player player, string link) =>
            context.Npcs
                .Include(n => n.NpcLink)
                .Include(n => n.NpcInfo)
                .Include(n => n.NpcDialogues)
                    .ThenInclude(nd => nd.Dialogue)
                .FirstOrDefault(n => n.NpcLink.Name == link && context.PlayerNpcs
                    .Any(pn => pn.PlayerId == player.Id && pn.NpcLinkId == n.NpcLinkId && pn.Phase == n.Phase));

        private Dialogue GetDialogueByLink(GsContext context, Player player, string link) =>
            context.Dialogues.FirstOrDefault(d => d.LinkName == link);

        private IEnumerable<Npc> GetNpcByDialogue(GsContext context, Player player, Dialogue dialogue) =>
            context.Npcs
                .Where(n => n.NpcDialogues.Any(nd => nd.DialogueId == dialogue.Id && nd.NpcId == n.Id) && context.PlayerNpcs
                    .Any(pn => pn.PlayerId == player.Id && pn.NpcLinkId == n.NpcLinkId && pn.Phase == n.Phase));
    }
}
