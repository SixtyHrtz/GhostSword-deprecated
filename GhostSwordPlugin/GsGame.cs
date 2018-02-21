using GhostSword;
using GhostSword.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Composition;

namespace GhostSwordPlugin
{
    [Export(typeof(Game))]
    public class GsGame : Game
    {
        private GsMessageHandler messageHandler;
        private GsRepository repository;

        public override IMessageHandler MessageHandler { get { return messageHandler; } }
        public override Repository Repository { get { return repository; } }

        public override DbContext GetContextInstance() => new GsContext();

        public GsGame()
        {
            repository = new GsRepository();
            messageHandler = new GsMessageHandler(repository);
        }
    }
}
