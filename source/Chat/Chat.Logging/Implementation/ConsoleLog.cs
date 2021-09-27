using Chat.Logging.Interfaces;
using System;

namespace Chat.Logging.Implementation
{
    /// <summary>
    /// Console Log
    /// </summary>
    public class ConsoleLog : ILog
    {
        ///<inheritdoc/>
        public void LogError(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] [ERROR] {message}");
        }

        ///<inheritdoc/>
        public void LogInfo(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
}