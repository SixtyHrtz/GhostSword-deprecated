using GhostSword.Enums;
using GhostSword.Types;
using System;
using System.Drawing;
using System.IO;

namespace GhostSword
{
    public class Debug
    {
        private string name;

        public Debug(string name) => this.name = name;

        public void Log(string message, LogType logType)
        {
            switch (logType)
            {
                case LogType.Log: Log(message); break;
                case LogType.Warning: LogWarning(message); break;
                case LogType.Error: LogError(message); break;
            }
        }

        public void Log(Data<Message> message)
        {
            if (message.IsValid)
                Log(message.Value.Text);
            else
                LogError(message.Error.Text);
        }

        public void Log(string message)
        {
            message = $"[{name} log] " + message;
            LogToFile(message);
            Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            message = $"[{name} warning] " + message;
            LogToFile(message);
            Console.WriteLine(message, Color.Orange);
        }

        public void LogError(string message)
        {
            message = $"[{name} error] " + message;
            LogToFile(message);
            Console.WriteLine(message, Color.Red);
        }

        private void LogToFile(string message)
        {
            var path = Environment.CurrentDirectory + "\\Log\\";
            Directory.CreateDirectory(path);

            var fileName = path + $"{DateTime.Now.ToString("dd-MM-yyyy")}.log";
            File.AppendAllText(fileName, message + Environment.NewLine);
        }
    }
}
