using GhostSword.Interfaces;

namespace GhostSword.Types
{
    public class AnswerMessage
    {
        public IUser User { get; set; }
        public Message Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public AnswerMessage(IUser user, Message message, Keyboard keyboard = null)
        {
            User = user;
            Message = message;
            Keyboard = keyboard;
        }
    }
}
