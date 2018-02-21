namespace GhostSword.Types
{
    public class Button
    {
        public string Text { get; set; }

        public Button(string text) => Text = text;

        public static implicit operator Button(string text) => new Button(text);
    }
}
