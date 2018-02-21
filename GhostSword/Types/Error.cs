namespace GhostSword.Types
{
    public class Error
    {
        public string Text { get; private set; }

        public Error(string text) => Text = text;

        public override string ToString() => Text;
    }
}
