using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace GhostSwordPlugin
{
    public class GsMessageHandler : BaseMessageHandler, IMessageHandler
    {
        private GsRepository gsRepository;

        public GsMessageHandler(Repository repository) => gsRepository = (GsRepository)repository;

        protected override void RegisterCommands()
        {
            RegisterDef("/start", gsRepository.LookAround);
            RegisterId("/npc", gsRepository.GetDialogues);
            RegisterId("/dial", gsRepository.GetDialogue);
            RegisterId("/place", gsRepository.BeginJourney);
            RegisterTwoId("/drop", gsRepository.DropItem);
            RegisterDef(GsResources.LookAround, gsRepository.LookAround);
            RegisterDef(GsResources.Backpack, gsRepository.InspectBackpack);
            RegisterDef(GsResources.Drop, gsRepository.GetDropItemList);
            RegisterDef(GsResources.Back, gsRepository.BackToPrevMenu);
        }

        private void RegisterDef(string input, Func<GsContext, Player, Message> function) =>
            Register(input, function);

        private void RegisterId(string input, Func<GsContext, Player, int, Message> function) =>
            Register(input, function);

        private void RegisterTwoId(string input, Func<GsContext, Player, int, int, Message> function) =>
            Register(input, function);

        public Data<Message> Invoke(DbContext context, IUser user, Command command)
        {
            var messageContext = new GsMessageContext((GsContext)context, (Player)user, command.Arguments);
            return Invoke(command, messageContext);
        }
    }
}
