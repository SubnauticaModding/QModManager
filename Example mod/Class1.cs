using QModManager;
using QModManager.API;
using System;

namespace Example_mod
{
    public static class Mod
    {
        public static void Patch()
        {
            Console.WriteLine("[TEST] Patched!");
            QModHooks.OnLoadEnd += () =>
            {
                Console.WriteLine("[TEST] Preparing to send message");
                QModAPI.SendMessage(QModAPI.GetMyMod(), "test", 1, true, "hello");
                Console.WriteLine("[TEST] Message sent!");
            };
        }
    }

    public class MessageRec : MessageReceiver
    {
        public override IQMod From { get; } = QModAPI.GetMyMod();

        public override void OnMessageReceived(IQMod from, string message, params object[] data)
        {
            Console.WriteLine("[TEST] Message received!");
            Console.WriteLine(from.DisplayName);
            Console.WriteLine(message);
            Console.WriteLine(data[0]);
            Console.WriteLine(data[1]);
            Console.WriteLine(data[2]);
        }
    }
}
