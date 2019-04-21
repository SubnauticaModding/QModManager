using System;
using System.Linq;
using System.Reflection;

namespace QModManager.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MessageReceiverAttribute : Attribute
    {
        public QMod Sender;

        public MessageReceiverAttribute(QMod sender)
        {
            Sender = sender ?? throw new ArgumentNullException("The provided mod is null!", nameof(sender));
        }
        public MessageReceiverAttribute(Assembly senderAssembly)
        {
            Sender = QModAPI.GetMod(senderAssembly, true, true) ?? throw new ArgumentException("The provided assembly is not a mod assembly!", nameof(senderAssembly));
        }
        public MessageReceiverAttribute(string senderID)
        {
            // Should we not want to throw an exception in case the mod is not present?
            Sender = QModAPI.GetMod(senderID, true, true) ?? throw new ArgumentException($"No mod matching the provided ID \"{senderID}\" was found!", nameof(senderID));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GlobalMessageRecieverAttribute : Attribute
    {
        public GlobalMessageRecieverAttribute() { }
    }

    public static partial class QModAPI
    {
        public static void SendMessage(QMod mod, string message, params object[] data)
        {
            if (mod == null) return;
            if (data == null) data = new object[] { };

            QMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            mod.LoadedAssembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.IsStatic)
                .Where(m => m.GetParameters().Length == 3)
                .Where(m => m.GetCustomAttributes(typeof(MessageReceiverAttribute), false)
                    .Where(a => ((MessageReceiverAttribute)a).Sender == caller)
                    .ToArray().Length > 0)
                .ForEach(m => m.Invoke(null, new object[] { caller, message, data }));
        }
        public static void SendMessage(Assembly modAssembly, string message, params object[] data)
            => SendMessage(GetMod(modAssembly, true, true), message, data);
        public static void SendMessage(string modID, string message, params object[] data)
            => SendMessage(GetMod(modID, true, true), message, data);

        public static void BroadcastMessage(string message, params object[] data)
        {
            if (data == null) data = new object[] { };

            QMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetMethods())
                .Where(m => m.IsStatic)
                .Where(m => m.GetParameters().Length == 3)
                .Where(m => m.GetCustomAttributes(typeof(GlobalMessageRecieverAttribute), false).Length > 0)
                .ForEach(m => m.Invoke(null, new object[] { caller, message, data }));
        }
    }
}