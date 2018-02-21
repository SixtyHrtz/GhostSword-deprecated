using Newtonsoft.Json;
using System.IO;

namespace GhostSword.Types
{
    public class DatabaseSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool? TrustedConnection { get; set; }

        public static DatabaseSettings Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DatabaseSettings>(json);
        }

        public Data<string> GetConnectionString()
        {
            var result = string.Empty;

            if (string.IsNullOrWhiteSpace(Server))
                return Data<string>.CreateError($"{Resources.ServerAddressNotFound}!");
            if (string.IsNullOrWhiteSpace(Database))
                return Data<string>.CreateError($"{Resources.DatabaseNameNotFound}!");

            result += $"Server={Server};Database={Database};";

            if (!string.IsNullOrWhiteSpace(UserId))
                result += $"User Id={UserId};";
            if (!string.IsNullOrWhiteSpace(Password))
                result += $"Password={Password};";

            if (TrustedConnection == null)
                result += "Trusted_Connection=False";
            else
                result += $"Trusted_Connection={TrustedConnection}";

            return Data<string>.CreateValid(result);
        }
    }
}
