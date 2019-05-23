/*
using QModManager.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QModManager.API
{
    /// <summary>
    /// A class which can is inherited to create a message receiver
    /// </summary>
    public abstract class MessageReceiver
    {
        /// <summary>
        /// The <see cref="IQMod"/> from which this class will receive messages
        /// </summary>
        public abstract IQMod From { get; }

        /// <summary>
        /// Creates a new message receiver which will receive messages from a provided mod <para/>
        /// You will need to specify a mod for the "<see langword="From"/>" property.
        /// </summary>
        public MessageReceiver() { }

        /// <summary>
        /// The method which will be called when a message is received
        /// </summary>
        /// <param name="from">The <see cref="IQMod"/> which sent the message</param>
        /// <param name="message">The message itself</param>
        /// <param name="data">Additional arguments</param>
        public abstract void OnMessageReceived(IQMod from, string message, params object[] data);
    }

    /// <summary>
    /// A class which can is inherited to create a global message receiver
    /// </summary>
    public abstract class GlobalMessageReceiver
    {
        /// <summary>
        /// Creates a new global message receiver
        /// </summary>
        public GlobalMessageReceiver() { }

        /// <summary>
        /// The method which will be called when a message is received
        /// </summary>
        /// <param name="from">The <see cref="IQMod"/> which sent the message</param>
        /// <param name="message">The message itself</param>
        /// <param name="data">Additional arguments</param>
        public abstract void OnMessageReceived(IQMod from, string message, params object[] data);
    }

    /// <summary>
    /// The base class for the QModManager API
    /// </summary>
    public partial class QModAPI : IQModAPI
    {
        #region Static

        /// <summary>
        /// Sends a message to a mod
        /// </summary>
        /// <param name="mod">The <see cref="IQMod"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        public static void SendMessage(IQMod mod, string message, params object[] data) 
            => Main.SendMessage(mod, message, data);
        /// <summary>
        /// Sends a message to a mod based on its assembly
        /// </summary>
        /// <param name="modAssembly">The <see cref="Assembly"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        public static void SendMessage(Assembly modAssembly, string message, params object[] data)
            => Main.SendMessage(modAssembly, message, data);
        /// <summary>
        /// Sends a message to a mod based on its ID
        /// </summary>
        /// <param name="modID">The id of the mod to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        public static void SendMessage(string modID, string message, params object[] data)
            => Main.SendMessage(modID, message, data);

        /// <summary>
        /// Broadcasts a message to all global message receivers
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        /// <param name="data">Additional arguments</param>
        public static void BroadcastMessage(string message, params object[] data) 
            => Main.BroadcastMessage(message, data);

        #endregion

        /// <summary>
        /// Sends a message to a mod
        /// </summary>
        /// <param name="mod">The <see cref="IQMod"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void IQModAPI.SendMessage(IQMod mod, string message, params object[] data)
        {
            if (mod == null) return;
            if (data == null) data = new object[] { };

            IQMod caller = GetMod(ReflectionHelper.CallingAssemblyByStackTrace(), true);

            if (mod.MessageReceivers == null || mod.MessageReceivers.Count < 1) return;

            if (mod.MessageReceivers.TryGetValue(caller, out List<MethodInfo> methods))
            {
                foreach (MethodInfo method in methods)
                {
                    method.Invoke(null, new object[] { caller, message, data });
                }
            }
        }
        /// <summary>
        /// Sends a message to a mod based on its assembly
        /// </summary>
        /// <param name="modAssembly">The <see cref="Assembly"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void IQModAPI.SendMessage(Assembly modAssembly, string message, params object[] data) 
            => SendMessage(GetMod(modAssembly, true, true), message, data);
        /// <summary>
        /// Sends a message to a mod based on its ID
        /// </summary>
        /// <param name="modID">The id of the mod to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void IQModAPI.SendMessage(string modID, string message, params object[] data) 
            => SendMessage(GetMod(modID, true, true), message, data);

        /// <summary>
        /// Broadcasts a message to all global message receivers
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        /// <param name="data">Additional arguments</param>
        void IQModAPI.BroadcastMessage(string message, params object[] data)
        {
            if (data == null) data = new object[] { };

            IQMod caller = GetMod(ReflectionHelper.CallingAssemblyByStackTrace(), true);

            Dictionary<IQMod, List<MethodInfo>> messageReceivers = GetAllMods()
                .SelectMany(m => m.MessageReceivers ?? new Dictionary<IQMod, List<MethodInfo>>() { })
                .ToDictionary(k => k.Key, v => v.Value);

            if (messageReceivers.Count < 1) return;

            if (messageReceivers.TryGetValue(QMod.QModManagerQMod, out List<MethodInfo> methods))
            {
                foreach (MethodInfo method in methods)
                {
                    method.Invoke(null, new object[] { caller, message, data });
                }
            }
        }
    }

    /// <summary>
    /// The base interface for the QModManager API <para/>
    /// Can be used for dependency injection
    /// </summary>
    public partial interface IQModAPI
    {
        /// <summary>
        /// Sends a message to a mod
        /// </summary>
        /// <param name="mod">The <see cref="IQMod"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void SendMessage(IQMod mod, string message, params object[] data);
        /// <summary>
        /// Sends a message to a mod based on its assembly
        /// </summary>
        /// <param name="modAssembly">The <see cref="Assembly"/> to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void SendMessage(Assembly modAssembly, string message, params object[] data);
        /// <summary>
        /// Sends a message to a mod based on its ID
        /// </summary>
        /// <param name="modID">The id of the mod to which the message will be sent</param>
        /// <param name="message">The message to send</param>
        /// <param name="data">Additional arguments</param>
        void SendMessage(string modID, string message, params object[] data);

        /// <summary>
        /// Broadcasts a message to all global message receivers
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        /// <param name="data">Additional arguments</param>
        void BroadcastMessage(string message, params object[] data);
    }
}
*/