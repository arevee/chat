namespace Chat.Protocol
{
    public enum Command
    {
        UserRegistration = 0x1,
        TextMessage = 0x2,
        Disconnect = 0x3,
        GetHistory = 0x4
    }
}