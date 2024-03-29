using ACE.Server.Mods;
using HarmonyLib;

namespace ACE.SimpleMod
{
    public class Mod : IHarmonyMod
    {
        //Point to your mod directory
    public static ModContainer Container => ModManager.GetModContainerByPath(Mod.ModPath);
        public static string ModPath = Path.Combine(ModManager.ModPath, "$safeprojectname$");

        //IDs are used by Harmony to separate multiple patches
        const string ID = "com.ACE.ACEmulator.$safeprojectname$";
        protected static Harmony Harmony { get; set; } = new(ID);

        private bool disposedValue;

        #region Initialize / Dispose (called by ACE)
        public void Initialize()
        {
            Harmony.PatchAllUncategorized();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Harmony.UnpatchAll(ID);
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
        #endregion
    }
}