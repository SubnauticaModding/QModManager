using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QModManager.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MessageReceiver : Attribute
    {
        public QMod Sender;

        public MessageReceiver(QMod sender)
        {
            Sender = sender ?? throw new ArgumentNullException("The provided mod is null!", nameof(sender));
        }
        public MessageReceiver(Assembly senderAssembly)
        {
            Sender = QModAPI.GetMod(senderAssembly, true, true) ?? throw new ArgumentException("The provided assembly is not a mod assembly!", nameof(senderAssembly));
        }
        public MessageReceiver(string senderID)
        {
            // Should we not want to throw an exception in case the mod is not present?
            Sender = QModAPI.GetMod(senderID, true, true) ?? throw new ArgumentException($"No mod matching the provided ID \"{senderID}\" was found!", nameof(senderID));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class GlobalMessageReciever : Attribute
    {
        public GlobalMessageReciever() { }
    }

    public partial class QModAPI : IQModAPI
    {
        public static void SendMessage(QMod mod, string message, params object[] data) 
            => Main.SendMessage(mod, message, data);
        public static void SendMessage(Assembly modAssembly, string message, params object[] data)
            => Main.SendMessage(modAssembly, message, data);
        public static void SendMessage(string modID, string message, params object[] data)
            => Main.SendMessage(modID, message, data);

        public static void BroadcastMessage(string message, params object[] data) => Main.BroadcastMessage(message, data);

        #region Non-static

        void IQModAPI.SendMessage(QMod mod, string message, params object[] data)
        {
            if (mod == null) return;
            if (data == null) data = new object[] { };

            QMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            if (mod.MessageReceivers.TryGetValue(caller, out List<MethodInfo> methods))
                foreach (MethodInfo method in methods)
                    method.Invoke(null, new object[] { caller, message, data });
        }
        void IQModAPI.SendMessage(Assembly modAssembly, string message, params object[] data) 
            => SendMessage(GetMod(modAssembly, true, true), message, data);
        void IQModAPI.SendMessage(string modID, string message, params object[] data) 
            => SendMessage(GetMod(modID, true, true), message, data);

        void IQModAPI.BroadcastMessage(string message, params object[] data)
        {
            if (data == null) data = new object[] { };

            QMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            if (GetAllMods().SelectMany(m => m.MessageReceivers)
                            .ToDictionary(k => k.Key, v => v.Value)
                            .TryGetValue(null, out List<MethodInfo> methods))
                foreach (MethodInfo method in methods)
                    method.Invoke(null, new object[] { caller, message, data });
        }

        #endregion
    }

    public partial interface IQModAPI
    {
        void SendMessage(QMod mod, string message, params object[] data);
        void SendMessage(Assembly modAssembly, string message, params object[] data);
        void SendMessage(string modID, string message, params object[] data);

        void BroadcastMessage(string message, params object[] data);
    }
}