using GhostSword.Interfaces;
using GhostSword.Types;
using Microsoft.EntityFrameworkCore;

namespace GhostSword
{
    public abstract class Game
    {
        public abstract IMessageHandler MessageHandler { get; }
        public abstract Repository Repository { get; }

        public abstract DbContext GetContextInstance();

        public Data<IUser> GetUser(DbContext context, IncomeMessage message) =>
            Repository.GetUser(context, message);

        public Data<Message> InvokeCommand(DbContext context, IUser user, Command command) =>
            MessageHandler.Invoke(context, user, command);

        public Data<Keyboard> GetKeyboard(DbContext context, IUser user) =>
            Repository.GetKeyboard(context, user);
    }
}
