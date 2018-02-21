using GhostSword.Enums;
using GhostSword.Types.Sequences;
using System;
using System.Collections.Generic;

namespace GhostSword.Types
{
    public class Command
    {
        private const char SLASH = '/';
        private const char UNDERLINE = '_';
        private const char SPACE = ' ';

        private string input;
        private string name;
        private CommandType type;
        private Argument[] arguments;

        public string Input { get { return input; } }
        public string Name { get { return name; } }
        public CommandType Type { get { return type; } }
        public Argument[] Arguments { get { return arguments; } }

        public Command(string value)
        {
            input = value;
            type = CommandType.None;

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{Resources.CommandInputIsNull}!");
        }

        public static Data<Command> TryParse(string value)
        {
            var command = TryParseInternal(value, out string error);

            if (command != null)
                return Data<Command>.CreateValid(command);
            else
                return Data<Command>.CreateError(error);
        }

        private static Command TryParseInternal(string value, out string error)
        {
            var command = new Command(value) { type = CommandType.Link };

            error = null;
            var sequence = new CharSequence(value);

            if (sequence.Current != SLASH)
            {
                command.name = command.input = value;
                command.type = CommandType.Common;
                return command;
            }

            if (TryTakeName(sequence, out string name))
            {
                var arguments = new List<Argument>();

                if (!TryTakeUnderlineArguments(sequence, arguments))
                {
                    error = $"{Resources.InvalidCommandFormat}: {value}";
                    return null;
                }

                if (!sequence.IsEos && sequence.Current == SPACE)
                {
                    sequence.Pop();
                    var arg = sequence.IsEos ? "" : sequence.PopRest();
                    AddArgument(arguments, arg);
                }

                command.name = name;
                command.arguments = arguments.ToArray();
            }
            else
            {
                error = $"{Resources.InvalidCommandFormat}: {value}";
                return null;
            }

            return command;
        }

        private static bool TryTakeName(CharSequence sequence, out string name)
        {
            name = string.Empty;
            name += sequence.Pop();

            while (!sequence.IsEos && IsCommandChar(sequence.Current))
                name += sequence.Pop();

            return name != string.Empty;
        }

        private static bool TryTakeUnderlineArguments(CharSequence sequence, List<Argument> arguments)
        {
            if (!sequence.IsEos && sequence.Current == UNDERLINE)
                if (!TryTakeArguments(sequence, arguments))
                    return false;

            return true;
        }

        private static bool TryTakeArguments(CharSequence sequence, List<Argument> arguments)
        {
            if (sequence.Current == UNDERLINE && sequence.IsEos)
                return false;

            if (sequence.Current == UNDERLINE)
                sequence.Pop();

            while (!sequence.IsEos && sequence.Current != UNDERLINE)
            {
                string arg = string.Empty;

                while (!sequence.IsEos && sequence.Current != UNDERLINE)
                {
                    if (sequence.Current == SPACE)
                        break;
                    arg += sequence.Pop();
                }

                if (arg != null)
                    AddArgument(arguments, arg);

                if (!sequence.IsEos)
                {
                    if (sequence.Current == SPACE)
                        break;
                    sequence.Pop();
                }
            }

            return true;
        }

        private static void AddArgument(List<Argument> arguments, string element)
        {
            if (int.TryParse(element, out int result))
                arguments.Add(new Argument(result));
            else
                arguments.Add(new Argument(element));
        }

        private static bool IsCommandChar(char ch) =>
            (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9');

        public static string Normalize(string value) => value.Trim();

        public override string ToString() => Input;
    }
}
