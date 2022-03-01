namespace SMLHelper.V2.Interfaces
{
    using Options.Attributes;

    /// <summary>
    /// Defines properties for <see cref="ModOptionEventAttribute"/> derivatives to implement for the purpose
    /// of holding metadata about events.
    /// </summary>
    public interface IModOptionEventAttribute
    {
        /// <summary>
        /// The name of the method to invoke.
        /// </summary>
        string MethodName { get; }
    }
}
