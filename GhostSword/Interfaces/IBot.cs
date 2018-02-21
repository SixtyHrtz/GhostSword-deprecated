using GhostSword.Types;

namespace GhostSword.Interfaces
{
    public interface IBot
    {
        bool IsReceiving { get; }

        void Register(IServerCore serverCore);
        Data<Message> SendText(IUser user, string text, Keyboard keyboard = null);
        Data<Message> Start();
        Data<Message> Stop();
    }
}
