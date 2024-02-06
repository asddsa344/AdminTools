using Exiled.API.Interfaces;
using Exiled.Loader;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Enable/Disable AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; }

        [Description("Should the tutorial class be in God Mode? Default: false")]
        public bool GodTuts { get; set; } = false;

        [Description("Extending Command use")]
        public bool qofieopf { get; set; } = false;
    }
}