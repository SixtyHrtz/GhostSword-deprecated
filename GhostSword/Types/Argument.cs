using GhostSword.Enums;

namespace GhostSword.Types
{
    public class Argument
    {
        public object Value { get; private set; }
        public ArgumentType Type { get; private set; }

        public Argument(int? argument)
        {
            Value = argument;
            Type = ArgumentType.Integer;
        }

        public Argument(string argument)
        {
            Value = argument;
            Type = ArgumentType.String;
        }

        public override string ToString() => $"{Resources.Value} {Value} {Resources.OfType} {Type}";
    }
}
