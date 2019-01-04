using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Crosstales.Common.EditorTask
{
    /// <summary>Base for adding the given define symbols to PlayerSettings define symbols.</summary>
    public abstract class BaseCompileDefines
    {
        protected static void setCompileDefines(string[] symbols)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (symbols != null)
            {
                bool found = true;

                foreach (string symbol in symbols)
                {
                    if (!definesString.Contains(symbol))
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    List<string> allDefines = definesString.Split(';').ToList();
                    allDefines.AddRange(symbols.Except(allDefines));

                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
                }
            }
        }
    }
}
// © 2018 crosstales LLC (https://www.crosstales.com)