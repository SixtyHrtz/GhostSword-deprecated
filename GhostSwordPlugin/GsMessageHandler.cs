using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using System;

namespace GhostSwordPlugin
{
    public class GsMessageHandler : BaseMessageHandler
    {
        private GsController controller;

        public GsMessageHandler(GsController controller) => this.controller = controller;

        protected override void RegisterCommands()
        {
            Register("/start", controller.LookAround);
            Register("/npc", controller.GetDialogues);
            Register("/dial", controller.GetDialogue);
            Register("/place", controller.BeginJourney);
            Register("/drop", controller.DropItem);
            Register("/rem", controller.RemoveItem);
            Register("/use", controller.UseItem);
            Register(GsResources.LookAround, controller.LookAround);
            Register(GsResources.Profile, controller.GetProfile);
            Register(GsResources.Backpack, controller.InspectInventory);
            Register(GsResources.Drop, controller.GetDropItemList);
            Register(GsResources.Back, controller.BackToPrevMenu);
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
