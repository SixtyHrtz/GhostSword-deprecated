using GhostSword.Interfaces;
using GhostSword.Types;
using System;
using System.Collections.Generic;
using System.Composition;

namespace GhostSwordPlugin
{
    [Export(typeof(IGame))]
    public class GsGame : IGame
    {
        public GsController Controller { get; set; }
        public GsMessageHandler MessageHandler { get; set; }

        public GsGame()
        {
            Controller = new GsController();
            MessageHandler = new GsMessageHandler(Controller);
        }

        public Tuple<IUser, Data<AnswerMessage>> GetAnswer(IncomeMessage message)
        {
            using (var session = new GsSession(this))
            {
                var user = session.GetUser(message);
                if (!user.IsValid)
                    return new Tuple<IUser, Data<AnswerMessage>>(null, Data<AnswerMessage>.CreateError(user.Error));

                var command = Command.TryParse(message.Text);
                if (!command.IsValid)
                    return new Tuple<IUser, Data<AnswerMessage>>(user.Value, Data<AnswerMessage>.CreateError(command.Error));

                var outputMessage = session.InvokeCommand(user.Value, command.Value);
                if (!outputMessage.IsValid)
                    return new Tuple<IUser, Data<AnswerMessage>>(user.Value, Data<AnswerMessage>.CreateError(outputMessage.Error));

                var keyboard = session.GetKeyboard(user.Value);

                var answer = new AnswerMessage(user.Value, outputMessage.Value, keyboard.Value);
                return new Tuple<IUser, Data<AnswerMessage>>(user.Value, Data<AnswerMessage>.CreateValid(answer));
            }
        }

        public List<AnswerMessage> GetEventsResults()
        {
            using (var session = new GsSession(this))
            {
                if (Controller.Locked)
                    return new List<AnswerMessage>();

                Controller.Lock();
                var answers = Controller.GetEventsResults(session.Context);
                Controller.Unlock();

                return answers;
            }
        }
    }
}
