using p3rpc.kotone.cutscenes.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using System.ComponentModel;

namespace p3rpc.kotone.cutscenes.Configuration
{
    public class Config : Configurable<Config>
    {

        [DisplayName("Enable Moonlight DayDream Opening?")]
        [Description("Select whether you want the Moonlight Daydream to show up after launching the game.")]
        [DefaultValue(true)]
        public bool OpeningEnabled { get; set; } = true;

        [DisplayName("Enable Cutscenes InGame?")]
        [Description("Select whether you want the ingame cutscenes to be replaced. Does not include the opening.")]
        [DefaultValue(true)]
        public bool GeneralScenesEnabled { get; set; } = true;

        [DisplayName("Velvet Room Attendant")]
        [Description("Select which Velvet Room Atthendant you want to appear in Cutscenes. Optionally if you have the Femc Mod selected you can select Auto Detect to copy over your preferences from there.")]
        [DefaultValue(VelvetAttendant.TheodoreBeta)]
        public VelvetAttendant VelvetTrue { get; set; } = VelvetAttendant.TheodoreBeta;
        public enum VelvetAttendant
        {
            TheodoreBeta,
            Elizabeth,
            AutoDetectBeta
        }

        [DisplayName("Log Level")]
        [Description("Select the logs you want to view.")]
        [DefaultValue(LogLevel.Standard)]
        public LogLevel LogTrue { get; set; } = LogLevel.Standard;
        public enum LogLevel
        {
            None,
            Standard,
            Debug
        }
    }

    /// <summary>
    /// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
    /// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
    /// </summary>
    public class ConfiguratorMixin : ConfiguratorMixinBase
    {
        // 
    }
}
