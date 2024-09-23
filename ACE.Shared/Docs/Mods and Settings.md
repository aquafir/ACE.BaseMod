## Mods

* `Mod` is the entry point for an individual mod.  They start and stop patches.
  * The simplest thing to do is to create a `BasicMod` with that type of mod's `IPatch` host
  * `BasicMod`
    * Implements `Initialize` and `Dispose` of `IHarmonyMod`
    * Constructor requires
      * Name, which is used to determine path/ID/container
      * `IPatch` to start/stop
    * Creates `Harmony` instance
    * On `Start`
      * Harmony patches uncategorized
      * Calls `Startup` on patches
      * Disables Mod on failure
    * On `Stop`
      * Harmony unpatches everything
      * Calls `Shutdown` on patches
    * Static `Instance` reference to self
    * `State` that tracks status of the mod when starting/loading settings/etc.
    * Creates and passes itself to an `IPatch` implementing `BasicPatch`
      * Calls `Init` / `Dispose`
* `PatchClass` is used in the template to implement an `IPatch` and inherits from `BasicPatch` 
* `BasicPatch<T>` consists of
  * `Init` / `Dispose`
    * Sets up / removes anything that doesn't change when settings reload
  * `Start` applies loads `Settings` and if successful calls
    * `OnStartSuccess`
    * `OnWorldOpen`, after waiting for the world to open
  * `Stop` removes things that might change with changes to the settings
  * `SettingsContainer<T>` that loads/watches `Settings` 
    * `SettingsChanged` called on reload
* `SettingsContainer<T>`
  * Handles serialization and reload logic for some `Settings`  class
  * `SettingsPath` points to the file the settings are serialized to
  * `LoadOrCreateAsync`  will attempt to load settings for a default of 10 seconds, and failing that will try to create them usingbetween saving/loading
    * `LoadSettingsAsync`
    * `SaveSettingsAsync(T settings)`
  * `JsonSettings` is the implementation used in the mod templates





