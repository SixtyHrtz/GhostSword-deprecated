using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;

namespace GhostSwordOnline.Console
{
    public class ConsoleManager
    {
        private readonly IServerCore serverCore;
        private readonly Debug debug;

        public ConsoleManager(IServerCore serverCore, Debug debug)
        {
            this.serverCore = serverCore;
            this.debug = debug;

            var handler = new CommandHandler(serverCore);
            while (handler.IsRunning)
            {
                var input = System.Console.ReadLine();
                var command = Command.TryParse(input);

                if (command.IsValid)
                    debug.Log(handler.Invoke(command.Value));
                else
                    debug.LogError(command.Error.Text);
            }
        }
    }
}
