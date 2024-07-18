* `SettingsContainer<T>`
  * Wraps some Settings data of type `T` 
  * `SettingsPath` points to the file the settings are serialized to
  * `CreateOrLoadAsync()` for default logic choosing between saving/loading
    * `CreateAsync()` if missing
    * `LoadAsync(string contents)` if found
  * Has FileWatcher / reload logic





* `IPatchClass` is an abstract class 
  * Contains SettingsContainer
  * Startup/Shutdown behavior
  * WorldOpen





SettingsContainer<Settings>





Settings : JsonSettings





## Mods

* `Mod` is the entry point for an individual mod.
  * Default is to create a `BasicMod` with that type of mod's `IPatch` host
* `BasicMod`
  * Implements `IHarmonyMod` of `Initialize`
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
  * Starts/stops PatchClass
  * Starts an `IPatchClass`
* `Patch`
  * Startup / Shutdown
* `BasicPatch<T>`
  * Contains a `SettingsContainer<T>` which creates/loads/reloads for settings `T` 
  * 

* PatchClass
  * Startup / Shutdown
  * Loads default Settings
  * 