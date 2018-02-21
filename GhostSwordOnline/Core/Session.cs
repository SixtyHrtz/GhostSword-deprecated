using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace GhostSwordOnline.Core
{
    public class Session : IDisposable
    {
        private bool disposed;
        private Game game;
        private DbContext context;

        public IUser User { get; private set; }

        public Session(Game game)
        {
            this.game = game;
            context = game.GetContextInstance();
        }

        public Data<AnswerMessage> GetAnswer(IncomeMessage message)
        {
            var user = game.GetUser(context, message);
            if (!user.IsValid)
                return Data<AnswerMessage>.CreateError(user.Error);

            User = user.Value;

            var command = Command.TryParse(message.Text);
            if (!command.IsValid)
                return Data<AnswerMessage>.CreateError(command.Error);

            var outputMessage = game.InvokeCommand(context, user.Value, command.Value);
            if (!outputMessage.IsValid)
                return Data<AnswerMessage>.CreateError(outputMessage.Error);

            var keyboard = game.GetKeyboard(context, user.Value);

            var answer = new AnswerMessage(user.Value, outputMessage.Value, keyboard.Value);
            return Data<AnswerMessage>.CreateValid(answer);
        }

        public List<AnswerMessage> GetEventsResults()
        {
            if (game.Repository.Locked)
                return new List<AnswerMessage>();

            game.Repository.Lock();
            var answers = game.Repository.GetEventsResults(context);
            game.Repository.Unlock();

            return answers;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                    context.Dispose();

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
