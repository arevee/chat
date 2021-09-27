using Chat.Protocol;
using NUnit.Framework;

namespace Tests.Chat.Protocol
{
    public class ProtocolTests
    {
        [Test]
        [TestCase(Command.UserRegistration, "Roma", new byte[] { 1, 4, 0, 82, 111, 109, 97 })]
        [TestCase(Command.TextMessage, "wow", new byte[] { 2, 3, 0, 119, 111, 119 })]
        [TestCase(Command.Disconnect, "", new byte[] { 3, 0, 0 })]
        [TestCase(Command.GetHistory, "", new byte[] { 4, 0, 0 })]
        public void ToBytesTest(Command command, string text, byte[] data)
        {
            var message = new Message(command, text);
            var messageBytes = message.ToBytes();
            Assert.AreEqual(messageBytes, data);
        }

        [Test]
        [TestCase(Command.UserRegistration, "Roma", new byte[] { 1, 4, 0, 82, 111, 109, 97 })]
        [TestCase(Command.TextMessage, "wow", new byte[] { 2, 3, 0, 119, 111, 119 })]
        [TestCase(Command.Disconnect, "", new byte[] { 3, 0, 0 })]
        [TestCase(Command.GetHistory, "", new byte[] { 4, 0, 0 })]
        public void FromBytesTest(Command command, string text, byte[] data)
        {
            var message = new Message(data);
            Assert.AreEqual(message.Command, command);
            Assert.AreEqual(message.Text, text);
        }
    }
}