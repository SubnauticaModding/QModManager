using UWE;

namespace SMLHelper.V2.Interfaces
{
    /// <summary>
    /// Handles WorldEntityDatabase.
    /// </summary>
    public interface IWorldEntityDatabaseHandler
    {
        /// <summary>
        /// Adds custom World Entity data to WorldEntityDatabase.
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="data"></param>
        void AddCustomInfo(string classId, WorldEntityInfo data); 
    }
}
