using GhostSword;
using GhostSword.Types;
using GhostSwordPlugin.Models;
using System.Reflection;

namespace GhostSwordPlugin
{
    public class GsMessageContext : ArgumentContext
    {
        private GsContext context;
        private Player player;

        public GsMessageContext(GsContext context, Player player, Argument[] arguments) : base(arguments)
        {
            this.context = context;
            this.player = player;
        }

        public override Data<object> TryResolve(ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType == typeof(GsContext))
                return Data<object>.CreateValid(context);
            if (parameterInfo.ParameterType == typeof(Player))
                return Data<object>.CreateValid(player);

            return base.TryResolve(parameterInfo);
        }
    }
}
