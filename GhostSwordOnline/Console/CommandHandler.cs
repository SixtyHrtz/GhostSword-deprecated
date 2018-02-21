using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using System;

namespace GhostSwordOnline.Console
{
    public class CommandHandler : BaseCommandHandler
    {
        private IServerCore serverCore;
        private bool isRunning = true;

        public bool IsRunning { get { return isRunning; } }

        public CommandHandler(IServerCore serverCore) => this.serverCore = serverCore;

        protected override void RegisterCommands()
        {
            Register("/server", serverCore.InvokeCommand);
            Register("/help", ShowCommandsList);
            Register("/exit", Exit);
        }

        private void Register(string input, Func<Message> function) =>
            Register(input, (Delegate)function);

        private void Register(string input, Func<string, Message> function) =>
            Register(input, (Delegate)function);

        public Data<Message> Invoke(Command command)
        {
            var context = new ArgumentContext(command.Arguments);
            return Invoke(command, context);
        }

        private Message ShowCommandsList()
        {
            var result = $"{Resources.ConsoleCommandList}:\n";
            foreach (string commandName in GetCommandsNames())
                result += $"{commandName}\n";

            return new Message(result);
        }

        private Message Exit()
        {
            isRunning = false;
            return new Message(Resources.AppExit);
        }
    }
}
