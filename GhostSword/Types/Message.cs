using GhostSword.Enums;
using System;
using System.IO;

namespace GhostSword.Types
{
    public class Message : IDisposable
    {
        public string Text { get; private set; }
        public FileStream Image { get; private set; }

        public MessageType Type { get; private set; }

        public Message(string text)
        {
            Text = text;
            Type = MessageType.Text;
        }

        public Message(FileStream fileStream, string text = null)
        {
            Text = text;
            Image = fileStream;
            Type = MessageType.Image;
        }

        public void Dispose() => Image.Dispose();
        public override string ToString() => Text;
    }
}
