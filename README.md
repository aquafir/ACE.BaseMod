### Samples

Included `Samples` have their own `Readme` and all except `CriticalOverride` have a short video demonstration:

* CriticalOverride and [HelloCommand](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/HelloCommand) are the simplest examples of patching or adding chat commands.
* [Spells](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/Spells) is a more sophisticated example (the most fun one), doing a handful of things with spells or UA combat.
* [Achievements](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/Achievements) currently just keeps a record of kills-by-id-by-player and gives a bonus every *n*th.  Eventually it'll be an example of persisting data without json.
* [CleaveTranspiler](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/CleaveTranspiler) is an example of modifying CIL
* [DiscordPlus](https://github.com/aquafir/ACE.BaseMod/tree/master/Samples/DiscordPlus) is a two-way chat relay that requires creating a bot and adding an assembly to ACE.



### Usage

Mods are made using [Harmony](https://harmony.pardeike.net/articles/intro.html#how-harmony-works) and require the `feat_mods` [branch of ACE](https://github.com/aquafir/ACE/tree/feat_mods).  Currently this is designed around a Windows environment with the standard installation path.

Here's an example of creating a `PostFix` that overrides the nether debuff rating bonus.

#### Overview

* Mods contain [patches](https://harmony.pardeike.net/articles/patching.html) to existing code.  
* Mods can be built outside of ACE (faster building) and be controlled while the server is live.
* Patches may be added and removed, and in the future ordered.  They can be added explicitly or using [attributes](https://harmony.pardeike.net/articles/annotations.html) describing the signature of what they're patching.
* They may appear before (prefix), after (postfix), or work by directly alter the instructions of a method as shown with the `CleaveTranspiler`.
* Since patches directly alter CIL instructions instead of using reflection they shouldn't cause a hit to performance.
* Harmony requires a static class to create patches.
* Some patches are easier than others.  If all logic can be done before (or instead of) or after a method, that's a great target.

#### Commands

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



#### Templates

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



### Todo

* More samples and a list of methods that might be interesting to patch.
* Possibly [Reverse patches](https://harmony.pardeike.net/articles/reverse-patching.html) to snapshot/restore any patched methods. `TryCreateILCopy` might be relevant.
* Good example for using [Finalizer patches](https://harmony.pardeike.net/articles/patching-finalizer.html) to eat tricky and inconsequential exceptions.
* Exposing capabilities of mods to other mods
  * `ModContainer`-based dependencies / incompatibilities.
* Fix things up for ACE-proper.
* Threading and all that CI/CD and non-Windows stuff I'm going to put off forever

