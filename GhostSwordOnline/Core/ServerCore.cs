using GhostSword;
using GhostSword.Interfaces;
using GhostSword.Types;
using GhostSwordOnline.Plugins;
using System;
using System.Timers;

namespace GhostSwordOnline.Core
{
    public class ServerCore : IServerCore, IDisposable
    {
        private Debug debug;
        private Timer timer;

        private BotPluginManager botManager;
        private GamePluginManager gameManager;

        public ServerCore() => debug = new Debug("Server");

        public Data<Message> Start()
        {
            PluginManager.LoadPlugins();

            botManager = new BotPluginManager();
            var botMessage = botManager.LoadObjects(this);
            if (!botMessage.IsValid)
                return botMessage;

            debug.Log(botMessage);

            gameManager = new GamePluginManager();
            var gameMessage = gameManager.LoadObjects(this);
            if (!gameMessage.IsValid)
                return gameMessage;

            debug.Log(gameMessage);

            timer = new Timer(1000);
            timer.Elapsed += OnTimer;
            timer.Start();

            return Data<Message>.CreateValid(new Message(Resources.ServerStarted));
        }

        public void OnMessage(IncomeMessage message, IBot bot)
        {
            debug.Log($"{message.Username}: {message.Text}");

            using (var session = new Session(gameManager.Game))
            {
                var answer = session.GetAnswer(message);
                if (!answer.IsValid)
                {
                    if (session.User != null)
                    {
                        bot.SendText(session.User, answer.Error.Text);
                        debug.Log($"{session.User.Username}: {answer.Error.Text}");
                    }
                    else
                        debug.Log(answer.Error.Text);

                    return;
                }

                bot.SendText(answer.Value.User, answer.Value.Message.Text, answer.Value.Keyboard);
                debug.Log($"{answer.Value.User.Username}: {answer.Value.Message}");
            }
        }

        public void OnError(string text) => debug.LogError(text);

        public void OnTimer(object sender, ElapsedEventArgs e)
        {
            using (var session = new Session(gameManager.Game))
            {
                var answers = session.GetEventsResults();
                foreach (var answer in answers)
                {
                    botManager.Bot.SendText(answer.User, answer.Message.Text, answer.Keyboard);
                    debug.Log($"{answer.User.Username}: {answer.Message}");
                }
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
