namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// Interface for event arguments for a <see cref="Options.ModOption"/>.
    /// </summary>
    public interface IModOptionEventArgs
    {
        /// <summary>
        /// The ID of the <see cref="Options.ModOption"/> this event corresponds to.
        /// </summary>
        string Id { get; }
    }
}
