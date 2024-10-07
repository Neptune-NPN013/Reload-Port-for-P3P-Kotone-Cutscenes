using p3rpc.kotone.cutscenes.Configuration;
using p3rpc.kotone.cutscenes.Template;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using static p3rpc.kotone.cutscenes.Configuration.Config;
using Ryo.Interfaces;

namespace p3rpc.kotone.cutscenes
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;

        /// <summary>
        /// Provides access to the IRyoApi
        /// </summary>
        private readonly IRyoApi _ryo;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;
            _ryo = GetDependency<IRyoApi>("Ryo");
            var process = Process.GetCurrentProcess();
            if (process.MainModule == null) throw new Exception($"[{_modConfig.ModName}] Could not get main module (this should never happen)");
            MovieManager();
        }

        private void MovieManager()
        {
            try
            {
                string path = _modLoader.GetDirectoryForModId(_modConfig.ModId);
                if (_configuration.OpeningEnabled) { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Opening Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Opening")); } }
                else { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Opening Disabled", System.Drawing.Color.Blue); } }
                if (_configuration.GeneralScenesEnabled) 
                { 
                    if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Cutscenes Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "General")); }
                    if (_configuration.VelvetTrue == VelvetAttendant.TheodoreBeta) { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Theodore Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Theodore")); } }
                    else if (_configuration.VelvetTrue == VelvetAttendant.Elizabeth) { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Elizabeth Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Elizabeth")); } }
                    else { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Attempting to AutoDetect Femc Config", System.Drawing.Color.Blue); } AutoDetectSelection(); }
                }
                else { if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Cutscenes Disabled", System.Drawing.Color.Blue); } }
            }
            catch (Exception Ex)
            {
                _logger.WriteLine("Failed to add movie paths to Ryo. The mod will not WORK");
            }
        }

        private void AutoDetectSelection()
        {
            try
            {
                string? femcdir = null;
                string? ReloadedModsDir = Path.GetDirectoryName(_modLoader.GetDirectoryForModId(_modConfig.ModId)); if (_configuration.LogTrue == LogLevel.Debug) { _logger.WriteLine("Reloaded Mods Directory Path: " + ReloadedModsDir, System.Drawing.Color.Aquamarine); }
                if (ReloadedModsDir is null)
                    return;
                string? ReloadedDir = Path.GetDirectoryName(ReloadedModsDir); if (_configuration.LogTrue == LogLevel.Debug) { _logger.WriteLine("Reloaded Directory Path: " + ReloadedDir, System.Drawing.Color.Aquamarine); };
                foreach (var dir in Directory.EnumerateDirectories(ReloadedModsDir))
                {
                    var femcDll = Path.Join(dir, "p3rpc.femc.dll");
                    if (File.Exists(femcDll))
                    {
                        femcdir = Path.GetFileName(dir);
                        break;
                    }
                }
                if (femcdir is not null)
                {
                    if (_configuration.LogTrue == LogLevel.Debug) { _logger.WriteLine("Femc Directory Name: " + femcdir, System.Drawing.Color.Aquamarine); }
                    if (!File.Exists(Path.Combine(ReloadedDir, "User", "Mods", femcdir, "Config.json")))
                    {
                        if (_configuration.LogTrue == LogLevel.Debug) { _logger.WriteLine("Unable to access the Femc Mod Config.", System.Drawing.Color.Red); }
                        if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Theodore Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Theodore")); }
                        return;
                    }
                    Theobro data = DeserializeFile<Theobro>(Path.Combine(ReloadedDir, "User", "Mods", femcdir, "Config.json"));
                    if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine(data.TheodorefromAlvinandTheChipmunks ? "Theodore Enabled" : "Elizabeth Enabled", System.Drawing.Color.Blue); }
                    if (data.TheodorefromAlvinandTheChipmunks) { _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Theodore")); }
                    else { _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Elizabeth")); }
                }
                else
                {
                    if (_configuration.LogTrue == LogLevel.Debug) { _logger.WriteLine("Unable to detect the Femc Mod. It is recommended to be installed along with this mod but not necessary.", System.Drawing.Color.Red); }
                    if (_configuration.LogTrue != LogLevel.None) { _logger.WriteLine("Theodore Enabled", System.Drawing.Color.Blue); _ryo.AddMoviePath(Path.Combine(_modLoader.GetDirectoryForModId(_modConfig.ModId), "Theodore")); }
                }
            }
            catch(Exception ex)
            {
                _logger.WriteLine("An error occured while trying to auto-detect the femc config. The scenes might be broken. Either disable AutoDetect or contact someone on the Official Discord/Github.", System.Drawing.Color.Red);
            }
        }

        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };

        public static T DeserializeFile<T>(string file)
        {
            byte[] fileBytes = File.ReadAllBytes(file);
            if (fileBytes.Length >= 3 && fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
            {
                fileBytes = fileBytes[3..]; // Skip the BOM
            }
            return JsonSerializer.Deserialize<T>(fileBytes, serializerOptions)
                   ?? throw new Exception("Failed to deserialize JSON.");
        }

        public class Theobro
        {
            public bool TheodorefromAlvinandTheChipmunks { get; set; }
        }

        private IControllerType GetDependency<IControllerType>(string modName) where IControllerType : class
        {
            var controller = _modLoader.GetController<IControllerType>();
            if (controller == null || !controller.TryGetTarget(out var target))
                throw new Exception($"[{_modConfig.ModName}] Could not get controller for \"{modName}\". This depedency is likely missing.");
            return target;
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion
    }
}