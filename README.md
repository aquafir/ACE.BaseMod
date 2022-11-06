*Early stages, hobbyist dev.  Happy to get advice/feedback*



Two templates exist: 

* `ACE.SimpleMod` has the basics needed by the `ModManager`.  
  * A class named `Mod` implementing `IHarmonyMod` is expected
  * `Meta.json` is used to control whether a mod is enabled, used for chat commands, or displayed
* `ACE.BaseMod` has:
  * Example `HarmonyPatch` in `PatchClass`
  * ``Settings` class that saves readable JSON to `Settings.json` and hot-reloads when saved
  * `Global usings` to keep some boilerplate includes out of the way



To create ACE mods either import the templates or:

* Open this repository in Visual Studio
* Project-->Export Template-->Export `ACE.BaseMod` or `ACE.SimpleMod`



The template defaults to building to `C:\ACE\Mods` which is where the `feat_mods` [branch](https://github.com/aquafir/ACE/tree/feat_mods) will look when starting.  That may be adjusted in ACE's `Config.js`:

>...
>"DatFilesDirectory": "c:\\ACE\\Dats\\",
>"ModsDirectory": "c:\\ACE\\Mods\\", 
>...



Mods can be managed in-game or via console using `/mod <verb> [part of mod name]` (skip the `/` in console).  

Verbs are:

* `list|l`
* `enable|e [name]`
* `disable|d [name]`
* `restart|r [name]`
* `toggle|t [name]`
* `find|f` shuts down all existing mods and looks for them again as if the server was starting
* Temporary convenience verbs are:
  * `settings|s [name]` will attempt to open the specific mods `Settings.json` with the default handler
  * `method|m Type MethodName` will print out some information about a method that may be helpful in creating a HarmonyPatch for it 




Included `Samples` have their own `Readme`:

* CriticalOverride and HelloCommand are the simplest examples of patching or adding chat commands.
* Achievements will eventually be using something besides Settings for managing state
* CleaveTranspiler is an example of modifying CIL
* DiscordPlus is a two-way chat relay that requires creating a bot and adding an assembly to ACE
* Spells is a bit more sophisticated mod that does some shenanigans with casting



### Todo

* Threading and all that CI/CD and non-Windows stuff I'm going to put off forever
* Possibly [Reverse patches](https://harmony.pardeike.net/articles/reverse-patching.html) to snapshot/restore any patched methods. `TryCreateILCopy` might be relevant.
* Good example for using [Finalizer patches](https://harmony.pardeike.net/articles/patching-finalizer.html) to eat tricky and inconsequential exceptions.
* Exposing capabilities of mods to other mods
  * `ModContainer`-based dependencies / incompatibilities.
