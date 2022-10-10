using ACE.Server.Network.GameMessages.Messages;

namespace Discord
{
    public class Mod : IHarmonyMod
    {
        //Todo: think about non-Windows environments
        //If Harmony is set to debug it creates a log on Desktop
        public const bool DEBUGGING = true;
        public const string ModPath = @"C:\ACE\Mods\Discord";
        const string ID = "com.ACE.ACEmulator.Discord";

        private Harmony _harmony;
        private bool disposedValue;

        public Harmony Harmony => _harmony;

        public void Initialize()
        {
            if (DEBUGGING)
            {
                Harmony.DEBUG = DEBUGGING;
                ModManager.Log($"Initializing {ID}...");
            }

            _harmony = new Harmony(ID);

            try
            {
                DiscordRelay.Initialize();

                //Patch GameMessageTurbineChat
                var chatConstructor = AccessTools.FirstConstructor(typeof(GameMessageTurbineChat), constructor => true);

                const int spacing = -40;
                var sb = new StringBuilder($"Constructor {chatConstructor.Name} found:\r\n{"Name",spacing}{"Type",spacing}{"Default",spacing}");

                foreach (var param in chatConstructor.GetParameters())
                    sb.AppendLine($"{param.Name,spacing}{param.ParameterType,spacing}{param.DefaultValue,spacing}");
                ModManager.Log(sb.ToString());

                var statsPrefix = SymbolExtensions.GetMethodInfo(() => PatchClass.Prefix);
                Harmony.Patch(chatConstructor, new HarmonyMethod(statsPrefix));

                //Patch everything in the mod with Harmony attributes
                //Harmony.PatchAll();
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to start.  Unpatching {ID}: {ex.Message}");
                Dispose();
            }
        }

        #region Dispose
        //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (DEBUGGING)
                        ModManager.Log($"Disposing {ID}...");

                    DiscordRelay.Shutdown();

                    //CustomCommands.Unregister();
                    Harmony.UnpatchAll(ID);

                    if (DEBUGGING)
                        ModManager.Log($"Unpatched {ID}...");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Mod()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        #region Dispose Async
        //https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
        //public async ValueTask DisposeAsync()
        //{
        //    await DisposeAsyncCore().ConfigureAwait(false);

        //    Dispose(disposing: false);
        //    #pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        //    GC.SuppressFinalize(this);
        //    #pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        //}

        //public async ValueTask DisposeAsyncCore()
        //{

        //}
        #endregion
        #endregion



        //public void Shutdown() => Dispose();

        //public void Dispose()
        //{
        //    if (DEBUGGING)
        //        ModManager.Log($"Disposing {ID}...");

        //    //CustomCommands.Unregister();
        //    Harmony.UnpatchAll(ID);

        //    if (DEBUGGING)
        //        ModManager.Log($"Unpatched {ID}...");
        //}
    }
}