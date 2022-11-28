using ACE.Database.Models.Shard;
using ACE.Database.Models.World;
using ACE.Entity.Enum.Properties;

namespace AccessDb
{
    [HarmonyPatch]
    public class PatchClass
    {
        #region Settings
        //private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(2);
        const int RETRIES = 10;

        public static Settings Settings = new();
        private static string settingsPath = Path.Combine(Mod.ModPath, "Settings.json");
        private static FileInfo settingsInfo = new(settingsPath);

        private static JsonSerializerOptions _serializeOptions = new()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }

        };

        private static void SaveSettings()
        {
            string jsonString = JsonSerializer.Serialize(Settings, _serializeOptions);

            if (!settingsInfo.RetryWrite(jsonString, RETRIES))
            {
                ModManager.Log($"Failed to save settings to {settingsPath}...", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
            }
        }

        private static void LoadSettings()
        {
            if (!settingsInfo.Exists)
            {
                ModManager.Log($"Creating {settingsInfo}...");
                SaveSettings();
            }
            else
                ModManager.Log($"Loading settings from {settingsPath}...");

            if (!settingsInfo.RetryRead(out string jsonString, RETRIES))
            {
                Mod.State = ModState.Error;
                return;
            }

            try
            {
                Settings = JsonSerializer.Deserialize<Settings>(jsonString, _serializeOptions);
            }
            catch (Exception)
            {
                ModManager.Log($"Failed to deserialize Settings: {settingsPath}", ModManager.LogLevel.Warn);
                Mod.State = ModState.Error;
                return;
            }
        }
        #endregion

        #region Start/Shutdown
        public static void Start()
        {
            //Need to decide on async use
            Mod.State = ModState.Loading;
            LoadSettings();

            if (Mod.State == ModState.Error)
            {
                ModManager.DisableModByPath(Mod.ModPath);
                return;
            }

            Mod.State = ModState.Running;
        }

        public static void Shutdown()
        {
            //if (Mod.State == ModState.Running)
            // Shut down enabled mod...

            if (Mod.State == ModState.Error)
                ModManager.Log($"Improper shutdown: {Mod.ModPath}", ModManager.LogLevel.Error);
        }
        #endregion
        [CommandHandler("do", AccessLevel.Admin, CommandHandlerFlag.None, -1)]
        public static void Do(Session session, params string[] parameters)
        {

            //DoShardStuff();
            //DoWorldStuff();
        }

        private static void DoWorldStuff()
        {
            using (var ctx = new WorldDbContext())
            {
                // Group creatures by type
                var query = from creature in ctx.Weenie
                            where creature.Type == (int)(WeenieType.Creature)
                            join cType in ctx.WeeniePropertiesInt on creature.ClassId equals cType.ObjectId
                            where cType.Type == (ushort)(PropertyInt.CreatureType)
                            select new
                            {
                                Name = creature.ClassName,
                                Id = creature.ClassId,
                                Type = cType.Value,
                            };

                var sb = new StringBuilder($"\n\n{"Name",-40}{"Type",-15}{"Type #",-10}\n");
                foreach (var group in query.ToList().GroupBy(x => x.Type).OrderBy(x => x.Count()))
                    sb.AppendLine($"{(CreatureType)group.Key,-40}{group.Key,-15}{group.Count(),-10}");

                ModManager.Log(sb.ToString());
            }
        }

        private static void DoShardStuff()
        {
            using (var context = new ShardDbContext())
            {
                var actNum = 1;
                var sb = new StringBuilder($"\nAccount {actNum}:\n");
                foreach (var character in context.Character.Where(c => c.AccountId == actNum).Select(s => new { s.Name, s.Id, s.TotalLogins }))
                {
                    sb.AppendLine($"  {character.Name} - {character.Id} - {character.TotalLogins}");
                }

                ModManager.Log(sb.ToString());
            }
        }

        #region Patches
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(Creature), nameof(Creature.GetDeathMessage), new Type[] { typeof(DamageHistoryInfo), typeof(DamageType), typeof(bool) })]
        //public static void PreDeathMessage(DamageHistoryInfo lastDamagerInfo, DamageType damageType, bool criticalHit, ref Creature __instance)
        //{
        //  ...
        //}
        #endregion
    }
}