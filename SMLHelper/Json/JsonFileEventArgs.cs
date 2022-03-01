using System;

namespace SMLHelper.V2.Json
{
    /// <summary>
    /// Contains basic information for a <see cref="JsonFile"/> event.
    /// </summary>
    public class JsonFileEventArgs : EventArgs
    {
        /// <summary>
        /// The instance of the <see cref="JsonFile"/> this event pertains to.
        /// </summary>
        public JsonFile Instance { get; }

        /// <summary>
        /// Instantiates a new <see cref="JsonFileEventArgs"/>.
        /// </summary>
        /// <param name="instance">The <see cref="JsonFile"/> instance the event pertains to.</param>
        public JsonFileEventArgs(JsonFile instance)
        {
            Instance = instance;
        }
    }
}
