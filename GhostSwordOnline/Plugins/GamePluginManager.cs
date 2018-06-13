using GhostSword.Interfaces;
using GhostSword.Types;
using System.Collections.Generic;

namespace GhostSwordOnline.Plugins
{
    public class GamePluginManager : PluginManager<IGame>
    {
        public IGame Game { get; private set; }

        public override string PluginType { get { return Resources.Game; } }

        public override Data<Message> InitObjects(IServerCore serverCore, List<IGame> objects)
        {
            if (objects.Count > 1)
                return Data<Message>.CreateError($"{Resources.FoundMoreThanOnePlugin} {OfType}. {Resources.NeedOnlyOnePlugin}");

            Game = objects[0];
            return Data<Message>.CreateValid(new Message($"{Resources.Plugin} {OfType} {Resources.SuccessfullyRegistered}"));
        }
    }
}
