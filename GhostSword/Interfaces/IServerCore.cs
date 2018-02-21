using GhostSword.Types;

namespace GhostSword.Interfaces
{
    public interface IServerCore
    {
        void Debug(Data<Message> message);
        void OnMessage(IncomeMessage message, IBot bot);
        void OnError(string text);

        Message InvokeCommand(string command);
    }
}
