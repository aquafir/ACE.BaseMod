using ACE.Server.Mod;
using HarmonyLib;

namespace ACE.SimpleMod
{
    public class Mod : IHarmonyMod
    {
        public const bool DEBUGGING = false;
        public const string ModPath = @"C:\ACE\Mods\$safeprojectname$";

        //IDs are used by Harmony to separate multiple patches
        const string ID = "com.ACE.ACEmulator.$safeprojectname$";
        private Harmony Harmony { get; set; } = new(ID);
        public static ModContainer Container { get; private set; }

        private bool disposedValue;

        public void Initialize()
        {
            if (DEBUGGING)
            {
                Harmony.DEBUG = DEBUGGING;
                ModManager.Log($"Initializing {ID}...");
            }

            Container = ModManager.GetModContainerByPath(ModPath);

            try
            {
                PatchClass.Start();

                //Patch everything in the mod with Harmony attributes
                Harmony.PatchAll();
            }
            catch (Exception ex)
            {
                ModManager.Log($"Failed to start.  Unpatching {ID}: {ex.Message}");
                Container?.Shutdown();
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

                    PatchClass.Shutdown();

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
    }
}