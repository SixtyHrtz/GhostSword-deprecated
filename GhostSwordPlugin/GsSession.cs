using GhostSword.Interfaces;
using GhostSword.Types;
using System;

namespace GhostSwordPlugin
{
    public class GsSession : IDisposable
    {
        private bool disposed;
        private GsGame game;

        public GsContext Context { get; set; }
        public IUser User { get; private set; }

        public GsSession(GsGame game)
        {
            this.game = game;
            Context = new GsContext();
        }

        public Data<IUser> GetUser(IncomeMessage message) =>
            game.Controller.GetUser(Context, message);

        public Data<Message> InvokeCommand(IUser user, Command command) =>
            game.MessageHandler.Invoke(Context, user, command);

        public Data<Keyboard> GetKeyboard(IUser user) =>
            game.Controller.GetKeyboard(Context, user);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    Context.Dispose();

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
