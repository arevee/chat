using System;
using System.Linq;
using System.Text;

namespace Chat.Protocol
{
    public class Message
    {
        public Command Command { get; }
        public string Text { get; }

        public Message(byte[] data)
        {
            if (data.Length >= 3)
            {
                Command = (Command)data[0];
                var textLength = BitConverter.ToInt16(data, 1);
                Text = Encoding.Default.GetString(data, sizeof(byte) + sizeof(short), textLength);
            }
        }

        public Message(Command command, string text)
        {
            Command = command;
            Text = text;
        }

        public byte[] ToBytes()
        {
            return new byte[1] { (byte)Command }.Concat(BitConverter.GetBytes((short)Text.Length)).Concat(Encoding.Default.GetBytes(Text ?? "")).ToArray();
        }

        public override string ToString()
        {
            return Text ?? "";
        }
    }
}