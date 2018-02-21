using Newtonsoft.Json;
using System.IO;

namespace GhostSword.Types
{
    public class BotSettings
    {
        public string Token { get; set; }

        public static BotSettings Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<BotSettings>(json);
        }
    }
}
