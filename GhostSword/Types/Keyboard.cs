namespace GhostSword.Types
{
    public class Keyboard
    {
        private Button[][] buttons;

        public Button[][] Buttons
        {
            get { return buttons; }
            set { buttons = value; }
        }

        public Keyboard(Button[] buttons) : this(new[] { buttons }) { }
        public Keyboard(Button[][] buttons) => this.buttons = buttons;
    }
}
