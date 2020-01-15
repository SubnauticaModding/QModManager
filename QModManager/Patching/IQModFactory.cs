using System.Collections.Generic;

namespace QModManager.Patching
{
    internal interface IQModFactory
    {
        /// <summary>
        /// Searches through all folders in the provided directory and returns an ordered list of mods to load.<para/>
        /// Mods that cannot be loaded will have an unsuccessful <see cref="QMod.Status"/> value.
        /// </summary>
        /// <param name="qmodsDirectory">The QMods directory</param>
        /// <returns>A new, sorted <see cref="List{QMod}"/> ready to be initialized or skipped.</returns>
        List<QMod> BuildModLoadingList(string qmodsDirectory);
    }
}