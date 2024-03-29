﻿using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using System;

namespace GhostSwordPlugin
{
    public class GsMessageHandler : BaseMessageHandler
    {
        private GsRepository repository;

        public GsMessageHandler(GsRepository repository) => this.repository = repository;

        protected override void RegisterCommands()
        {
            Register("/start", repository.LookAround);
            Register("/npc", repository.GetDialogues);
            Register("/dial", repository.GetDialogue);
            Register("/place", repository.BeginJourney);
            Register("/drop", repository.DropItem);
            Register("/rem", repository.RemoveItem);
            Register("/use", repository.UseItem);
            Register(GsResources.LookAround, repository.LookAround);
            Register(GsResources.Backpack, repository.InspectInventory);
            Register(GsResources.Drop, repository.GetDropItemList);
            Register(GsResources.Back, repository.BackToPrevMenu);
        }

        private void Register(string input, Func<GsContext, Player, Message> function) => base.Register(input, function);
        private void Register(string input, Func<GsContext, Player, string, Message> function) => base.Register(input, function);
        private void Register(string input, Func<GsContext, Player, uint, Message> function) => base.Register(input, function);
        private void Register(string input, Func<GsContext, Player, uint, uint, Message> function) => base.Register(input, function);

        public Data<Message> Invoke(GsContext context, IUser user, Command command)
        {
            var messageContext = new GsMessageContext(context, (Player)user, command.Arguments);
            return Invoke(command, messageContext);
        }
    }
}
