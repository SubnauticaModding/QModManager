using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QModManager
{
    class Options
    {
        [Option("SubnauticaDirectory", Required = false, HelpText = "Full path of the Subnautica folder located inside steamapps/common")]
        public string SubnauticaDirectory { get; set; }
    }
}
