using GhostSword.Interfaces;
using GhostSword.Types;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GhostSwordPlugin
{
    [Export(typeof(IBot))]
    public class TelegramBot : IBot
    {
        private TelegramBotClient client;

        public IServerCore ServerCore { get; private set; }
        public bool IsReceiving { get { return client.IsReceiving; } }

        public void Register(IServerCore serverCore) => ServerCore = serverCore;

        public Data<Message> Start()
        {
            try
            {
                var settings = BotSettings.Load("telegram.json");

                client = new TelegramBotClient(settings.Token);
                client.OnMessage += OnMessage;
                client.OnReceiveError += OnReceiveError;

                client.StartReceiving();
                return Data<Message>.CreateValid(new Message(GsResources.BotStarted));
            }
            catch (Exception e)
            {
                return Data<Message>.CreateError($"{GsResources.BotExecutionFailed}:\n{e.Message}");
            }
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.TextMessage)
                return;

            var chat = message.Chat;
            var incomeMessage = new IncomeMessage(
                chat.Id, chat.Username, chat.FirstName, chat.LastName, message.Text);

            ServerCore.OnMessage(incomeMessage, this);
        }

        private void OnReceiveError(object sender, ReceiveErrorEventArgs e) =>
            ServerCore.OnError(e.ApiRequestException.Message);

        private IReplyMarkup ConvertKeyboard(Keyboard keyboard)
        {
            if (keyboard == null)
                return null;

            var buttons = keyboard.Buttons
                .Select(x => x
                    .Select(y => new Telegram.Bot.Types.KeyboardButton(y.Text))
                    .ToArray())
                .ToArray();
            return new ReplyKeyboardMarkup(buttons, true);
        }

        public Data<Message> SendText(IUser user, string text, Keyboard keyboard = null) =>
            SendText(user, text, ConvertKeyboard(keyboard)).Result;

        public async Task<Data<Message>> SendText(IUser user, string text, IReplyMarkup replyMarkup = null)
        {
            try
            {
                if (replyMarkup == null)
                    await client.SendTextMessageAsync(user.UserId, text, ParseMode.Html);
                else
                    await client.SendTextMessageAsync(user.UserId, text, ParseMode.Html, replyMarkup: replyMarkup);

                return Data<Message>.CreateValid(new Message(text));
            }
            catch (Exception ex)
            {
                return Data<Message>.CreateError(ex.InnerException.Message);
            }
        }

        public Data<Message> Stop()
        {
            try
            {
                client.StopReceiving();
                return Data<Message>.CreateValid(new Message(GsResources.BotStopped));
            }
            catch (Exception e)
            {
                return Data<Message>.CreateError($"{GsResources.BotStoppingFailed}:\n{e.Message}");
            }
        }
    }
}
