using GhostSword.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GhostSword
{
    public abstract class BaseCommandHandler
    {
        private Dictionary<string, Delegate> commands;
        private Dictionary<string, ParameterInfo[]> commandParameters;
        private bool initialized = false;

        protected virtual int DefaultParamCount { get { return 0; } }

        protected BaseCommandHandler()
        {
            commands = new Dictionary<string, Delegate>();
            commandParameters = new Dictionary<string, ParameterInfo[]>();
        }

        private void Initialize()
        {
            RegisterCommands();
            initialized = true;
        }

        protected abstract void RegisterCommands();

        protected void Register(string input, Delegate function)
        {
            commands[Command.Normalize(input)] = function;
            commandParameters[Command.Normalize(input)] = function.Method.GetParameters();
        }

        protected Data<Message> Invoke(Command command, ArgumentContext context)
        {
            if (!initialized)
                Initialize();

            if (!commands.ContainsKey(command.Name) || !commandParameters.ContainsKey(command.Name))
                return Data<Message>.CreateError($"{Resources.CommandNotRegistered}: {command.Input}");

            var arguments = GetArguments(command, context);
            if (!arguments.IsValid)
                return Data<Message>.CreateError($"{arguments.Error.Text}: {command.Input}");

            var function = commands[command.Name];
            var message = Data<Message>.CreateValid(function.DynamicInvoke(arguments.Value) as Message);

            return message ?? Data<Message>.CreateError($"{Resources.MethodReturnsNothing}: {command.Input}");
        }

        private Data<object[]> GetArguments(Command command, ArgumentContext context)
        {
            var arguments = new List<object>();
            var parameters = commandParameters[command.Name];

            if (context.ArgumentCount < parameters.Length - DefaultParamCount)
                return Data<object[]>.CreateError(Resources.NotEnoughArguments);
            if (context.ArgumentCount > parameters.Length - DefaultParamCount)
                return Data<object[]>.CreateError(Resources.TooManyArguments);

            foreach (ParameterInfo parameter in parameters)
            {
                var value = context.TryResolve(parameter);
                if (!value.IsValid)
                    return Data<object[]>.CreateError(value.Error);

                arguments.Add(value.Value);
            }

            return Data<object[]>.CreateValid(arguments.ToArray());
        }

        protected List<string> GetCommandsNames() => new List<string>(commands.Keys);
    }
}
