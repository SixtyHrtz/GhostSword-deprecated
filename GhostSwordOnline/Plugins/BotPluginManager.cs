using GhostSword.Interfaces;
using GhostSword.Types;
using System.Collections.Generic;

namespace GhostSwordOnline.Plugins
{
    public class BotPluginManager : PluginManager<IBot>
    {
        public IBot Bot { get; private set; }

        public override string PluginType { get { return Resources.Bot; } }

        public override Data<Message> InitObjects(IServerCore serverCore, List<IBot> objects)
        {
            if (objects.Count > 1)
                return Data<Message>.CreateError($"{Resources.FoundMoreThanOnePlugin} {OfType}. {Resources.NeedOnlyOnePlugin}");

            var bot = objects[0];
            bot.Register(serverCore);

            var message = bot.Start();
            if (!message.IsValid)
                return message;

            serverCore.Debug(message);

            Bot = bot;
            return Data<Message>.CreateValid(new Message($"{Resources.Plugin} {OfType} {Resources.SuccessfullyRegistered}"));
        }
    }
}