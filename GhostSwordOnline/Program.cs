using GhostSword;
using GhostSwordOnline.Console;
using GhostSwordOnline.Core;

namespace GhostSwordOnline
{
    public class Program
    {
        public static void Main()
        {
            var debug = new Debug("Console");

            using (var serverCore = new ServerCore())
            {
                debug.Log(serverCore.Start());
                new ConsoleManager(serverCore, debug);
            }
        }
    }
}
