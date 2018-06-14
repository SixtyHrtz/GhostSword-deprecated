using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordOnline.Plugins;
using System;
using System.Timers;

namespace GhostSwordOnline
{
    public class ServerCore : IServerCore, IDisposable
    {
        private Debug debug;
        private Timer timer;
        private IGame game;

        private BotPluginManager botManager;

        public ServerCore() => debug = new Debug("Server");

        public Data<Message> Start()
        {
            PluginManager.LoadPlugins();

            var gameManager = new GamePluginManager();
            var gameMessage = gameManager.LoadObjects(this);
            if (!gameMessage.IsValid)
                return gameMessage;

            debug.Log(gameMessage);
            game = gameManager.Game;

            botManager = new BotPluginManager();
            var botMessage = botManager.LoadObjects(this);
            if (!botMessage.IsValid)
                return botMessage;

            debug.Log(botMessage);

            timer = new Timer(1000);
            timer.Elapsed += OnTimer;
            timer.Start();

            return Data<Message>.CreateValid(new Message(Resources.ServerStarted));
        }

        public void OnMessage(IncomeMessage message, IBot bot)
        {
            debug.Log($"{message.Username}: {message.Text}");

            var answer = game.GetAnswer(message);
            var user = answer.Item1;
            var answerMsg = answer.Item2;

            if (!answerMsg.IsValid)
            {
                if (user != null)
                {
                    bot.SendText(user, answerMsg.Error.Text);
                    debug.Log($"{user.Username}: {answerMsg.Error.Text}");
                }
                else
                    debug.Log(answerMsg.Error.Text);

                return;
            }

            bot.SendText(answerMsg.Value.User, answerMsg.Value.Message.Text, answerMsg.Value.Keyboard);
            debug.Log($"{answerMsg.Value.User.Username}: {answerMsg.Value.Message}");
        }

        public void OnError(string text) => debug.LogError(text);

        public void OnTimer(object sender, ElapsedEventArgs e)
        {
            var answers = game.GetEventsResults();
            foreach (var answer in answers)
            {
                botManager.Bot.SendText(answer.User, answer.Message.Text, answer.Keyboard);
                debug.Log($"{answer.User.Username}: {answer.Message}");
            }
        }

        public void Debug(Data<Message> message) => debug.Log(message);

        public Message InvokeCommand(string command)
        {
            switch (command)
            {
                case "start": return Start().Value;
                case "stop": return Stop().Value;
                default: return new Message($"{Resources.CommandNotFound}: {command}");
            }
        }

        public Data<Message> Stop()
        {
            foreach (var bot in botManager.Objects)
                if (!bot.IsReceiving)
                    debug.Log(Resources.BotAlreadyStopped);

            timer.Stop();

            var message = botManager.Bot.Stop();
            if (!message.IsValid)
                return message;
            debug.Log(message);

            return Data<Message>.CreateValid(new Message(Resources.ServerStopped));
        }

        public void Dispose() => Stop();
    }
}
