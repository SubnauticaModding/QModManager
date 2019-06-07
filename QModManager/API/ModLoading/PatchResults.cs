namespace QModManager.API.ModLoading
{
    /// <summary>
    /// An optional value that can be returned from a mod patching method.
    /// </summary>
    public enum PatchResults
    {
        /// <summary>
        /// The patch method ran successfully.<para/>
        /// A method that returns <seealso cref="void"/> and does not through an exception will default to this value.
        /// </summary>
        OK = 0,

        /// <summary>
        /// The patch method encountered an error.<para/>
        /// Execution of any further patch methods this mod implements will be skipped.
        /// </summary>
        Error = 1,

        /// <summary>
        /// Informs that the mod author has requested that patching be halted for this mod.<para/>
        /// Execution of any further patch methods this mod implements will be skipped.<para/>
        /// This doesn't necessarily imply that and error happened, so it will be handled silently.
        /// </summary>
        ModderCanceled = 2,
    }
}
