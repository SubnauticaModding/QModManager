using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QModManager.API
{
    public abstract class MessageReceiver
    {
        public IQMod From;

        public MessageReceiver(IQMod from)
        {
            if (from == null)
                throw new ArgumentNullException("from", "The provided mod is null!");

            From = from;
        }

        public MessageReceiver(Assembly fromAssembly)
        {
            if (fromAssembly == null)
                throw new ArgumentNullException(nameof(fromAssembly), "The provided assembly is null!");

            IQMod mod = QModAPI.GetMod(fromAssembly, true, true);

            if (mod == null)
                throw new ArgumentException("The provided assembly is not a mod assembly!", nameof(fromAssembly));

            From = mod;
        }

        public MessageReceiver(string fromID)
        {
            if (string.IsNullOrEmpty(fromID))
                throw new ArgumentNullException(nameof(fromID), "The provided ID is null or empty!");

            IQMod mod = QModAPI.GetMod(fromID, true, true);

            if (mod == null)
                throw new ArgumentException("No mod matching the provided ID was found!");

            From = mod;
        }

        public abstract void OnMessageReceived(IQMod from, string message, params object[] data);
    }

    public abstract class GlobalMessageReceiver
    {
        public GlobalMessageReceiver() { }

        public abstract void OnMessageReceived(IQMod from, string message, params object[] data);
    }

    public partial class QModAPI : IQModAPI
    {
        public static void SendMessage(IQMod mod, string message, params object[] data) 
            => Main.SendMessage(mod, message, data);
        public static void SendMessage(Assembly modAssembly, string message, params object[] data)
            => Main.SendMessage(modAssembly, message, data);
        public static void SendMessage(string modID, string message, params object[] data)
            => Main.SendMessage(modID, message, data);

        public static void BroadcastMessage(string message, params object[] data) => Main.BroadcastMessage(message, data);

        #region Non-static

        void IQModAPI.SendMessage(IQMod mod, string message, params object[] data)
        {
            if (mod == null) return;
            if (data == null) data = new object[] { };

            IQMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            if (mod.MessageReceivers == null || mod.MessageReceivers.Count < 1) return;

            if (mod.MessageReceivers.TryGetValue(caller, out List<MethodInfo> methods))
            {
                foreach (MethodInfo method in methods)
                {
                    method.Invoke(null, new object[] { caller, message, data });
                }
            }
        }
        void IQModAPI.SendMessage(Assembly modAssembly, string message, params object[] data) 
            => SendMessage(GetMod(modAssembly, true, true), message, data);
        void IQModAPI.SendMessage(string modID, string message, params object[] data) 
            => SendMessage(GetMod(modID, true, true), message, data);

        void IQModAPI.BroadcastMessage(string message, params object[] data)
        {
            if (data == null) data = new object[] { };

            IQMod caller = GetMod(Assembly.GetCallingAssembly(), true);

            Dictionary<IQMod, List<MethodInfo>> messageReceivers = GetAllMods()
                .SelectMany(m => m.MessageReceivers ?? new Dictionary<IQMod, List<MethodInfo>>() { })
                .ToDictionary(k => k.Key, v => v.Value);

            if (messageReceivers.Count < 1) return;

            if (messageReceivers.TryGetValue(null, out List<MethodInfo> methods))
            {
                foreach (MethodInfo method in methods)
                {
                    method.Invoke(null, new object[] { caller, message, data });
                }
            }
        }

        #endregion
    }

    public partial interface IQModAPI
    {
        void SendMessage(IQMod mod, string message, params object[] data);
        void SendMessage(Assembly modAssembly, string message, params object[] data);
        void SendMessage(string modID, string message, params object[] data);

        void BroadcastMessage(string message, params object[] data);
    }
}