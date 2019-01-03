using System;

namespace QModManager
{
    internal class LanguageLines
    {
        internal static class General
        {
            internal static readonly LanguageLines PressAnyKey = new string[]
            {
                "",
                "Press any key to exit...",
            };
            internal static readonly LanguageLines ExceptionCaught = "EXCEPTION CAUGHT!";
        }
        internal static class Executable
        {
            internal static readonly LanguageLines AssemblyMissing = new string[]
            {
                "Could not find the assembly file.",
                "Please make sure you have installed QModManager in the right folder.",
                "If the problem persists, open a bug report on NexusMods or an issue on GitHub",
            };
            internal static readonly LanguageLines ExeMissing = new string[]
            {
                "Could not find any game to patch!",
                "An assembly file was found, but no executable was detected.",
                "Please make sure you have installed QModManager in the right folder.",
                "If the problem persists, open a bug report on NexusMods or an issue on GitHub",
            };
            internal static readonly LanguageLines Installing = "Installing QModManager...";
            internal static readonly LanguageLines AlreadyInstalled = new string[]
            {
                "QModManager is already installed!",
                "Skipping installation",
            };
            internal static readonly LanguageLines Uninstalling = "Uninstalling QModManager...";
            internal static readonly LanguageLines AlreadyUninstalled = new string[]
            {
                "QModManager is already uninstalled!",
                "Skipping uninstallation",
            };
            internal static readonly LanguageLines AskInstall = "No patch detected, install? [Y/N] > ";
            internal static readonly LanguageLines AskUninstall = "Patch installed, remove? [Y/N] > ";
        }
        internal static class Dialog
        {
            internal static readonly LanguageLines DefaultLeft = "See Log";
            internal static readonly LanguageLines DefaultRight = "Close";
        }
        internal static class Injector
        {
            internal static readonly LanguageLines AlreadyInjected = new string[]
            {
                "Tried to install, but it was already injected",
                "Skipping installation",
            };
            internal static readonly LanguageLines Installed = new string[]
            {
                "",
                "QModManager was installed successfully",
            };
            internal static readonly LanguageLines NotInjected = new string[]
            {
                "Tried to uninstall, but patch was not present",
                "Skipping uninstallation",
            };
            internal static readonly LanguageLines Uninstalled = new string[]
            {
                "",
                "QModManager was uninstalled successfully",
            };
            internal static readonly LanguageLines BackupMissing = new string[]
            {
                "",
                "Cannot uninstall, file 'Assembly-CSharp-qoriginal.dll' is missing",
                "To uninstall, you will need to verify game contents on steam",
            };
        }
        internal static class Patcher
        {
            internal static readonly LanguageLines CalledMultipleTimes = "\nQMOD WARN: Patch method was called multiple times!";
            internal static readonly LanguageLines LoadedMods = "\nLoaded mods:\n";
            internal static readonly LanguageLines CouldNotBeLoaded = "\nQMOD ERROR: The following mods could not be loaded:";
            internal static readonly LanguageLines EntryMethodMissing = "ERROR! No EntryMethod specified for mod ";
            internal static readonly LanguageLines CannotParseEntryMethod1 = "ERROR! Could not parse entry method ";
            internal static readonly LanguageLines CannotParseEntryMethod2 = " for mod ";
            internal static readonly LanguageLines InvokingEntryMethodFailed1 = "ERROR! Invoking the specified entry method ";
            internal static readonly LanguageLines InvokingEntryMethodFailed2 = " failed for mod ";
            internal static readonly LanguageLines UnexpectedError = "ERROR! An unexpected error occurred while trying to load mod: ";
            internal static readonly LanguageLines SortingErrorLoop = new string[]
            {
                "\nQMOD ERROR: There was en error while sorting the following mods!",
                "Please check the 'LoadAfter' and 'LoadBefore' properties of these mods!\n",
            };
            internal static readonly LanguageLines SortingErrorLoopSeparator = " -> ";
            internal static readonly LanguageLines MissingDependencies = "\nQMOD ERROR: The following mods were not loaded due to missing dependencies!\n";
            internal static readonly LanguageLines MissingDependenciesPrefix = " (missing: ";
            internal static readonly LanguageLines MissingDependenciesSeparator = ", ";
            internal static readonly LanguageLines MissingDependenciesSuffix = ")";
            internal static readonly LanguageLines ErroredModsDisplayPrefix = "The following mods could not be loaded: ";
            internal static readonly LanguageLines ErroredModsDisplaySeparator = ", ";
            internal static readonly LanguageLines ErroredModsDisplaySuffix = ". Check the log for details.";
            internal static readonly LanguageLines NewVersionDisplayPrefix = "There is a newer version of QModManager available: ";
            internal static readonly LanguageLines NewVersionDisplayCurrent = " (current version: ";
            internal static readonly LanguageLines NewVersionDisplaySuffix = ")";

        }
        internal static class QMod
        {
            internal static readonly LanguageLines DeserializationFailed = "ERROR! mod.json deserialization failed!";
        }
        internal static class VersionCheck
        {
            internal static readonly LanguageLines ErrorStringNull = "Could not get latest version! (versionStr is null)";
            internal static readonly LanguageLines ErrorWrapperNull = "Could not get latest version! (wrapper is null)";
            internal static readonly LanguageLines ErrorWrapperVersionNull = "Could not get latest version! (wrapper.version is null)";
            internal static readonly LanguageLines ErrorVersionNull = "Could not get latest version! (version is null)";
            internal static readonly LanguageLines Cancelled = "CANCELLED";
        }
        
        internal string[] text;

        internal LanguageLines() { }
        internal string this[int i]
        {
            get
            {
                return text[i];
            }
        }
        public static implicit operator LanguageLines(string[] array)
        {
            return new LanguageLines
            {
                text = array,
            };
        }
        public static implicit operator LanguageLines(string str)
        {
            return new LanguageLines
            {
                text = new string[] { str }
            };
        }
        public static implicit operator string(LanguageLines lines)
        {
            string s = "";
            for (int i = 0; i < lines.text.Length; i++)
            {
                s += lines.text[i];
                if (i + 1 != lines.text.Length) s += "\n";
            }
            return s;
        }
    }
}
