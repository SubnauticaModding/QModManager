using System;

namespace SMLHelper.V2.Json
{
    /// <summary>
    /// Contains basic information for a <see cref="ConfigFile"/> event.
    /// </summary>
    public class ConfigFileEventArgs : EventArgs
    {
        /// <summary>
        /// The instance of the <see cref="ConfigFile"/> this event pertains to.
        /// </summary>
        public ConfigFile Instance { get; }

        /// <summary>
        /// Instantiates a new <see cref="ConfigFileEventArgs"/>.
        /// </summary>
        /// <param name="instance">The <see cref="ConfigFile"/> instance the event pertains to.</param>
        public ConfigFileEventArgs(ConfigFile instance)
        {
            Instance = instance;
        }
    }
}
