Check out the [wiki to get started](https://github.com/aquafir/ACE.BaseMod/wiki/Getting-Started), or the [spreadsheet](https://docs.google.com/spreadsheets/u/1/d/16XrOSBW195BlrUnsb0Ax4jTRkj25WG0rXxOIz3cWrU8/edit#gid=2104144189) to find templates, samples, or things to patch.



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



#### Todo

* Mod repository
* Support selective adding of commands with attributes
* Possibly [Reverse patches](https://harmony.pardeike.net/articles/reverse-patching.html) to snapshot/restore any patched methods. `TryCreateILCopy` might be relevant.
* Good example for using [Finalizer patches](https://harmony.pardeike.net/articles/patching-finalizer.html) to eat tricky and inconsequential exceptions.
* Exposing capabilities of mods to other mods
  * `ModContainer`-based dependencies / incompatibilities.
* Threading and all that CI/CD and non-Windows stuff I'm going to put off forever

