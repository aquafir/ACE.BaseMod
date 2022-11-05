*Still in very early stages, but I'm taking a short break to finish other things.*

To create ACE mods:

* Open this repository in Visual Studio
* Project-->Export Template-->Export `ACE.BaseMod`

The template defaults to building to `C:\ACE\Mods` which is where the `feat_mods` [branch](https://github.com/aquafir/ACE/tree/feat_mods) will look when starting.

Mods can be managed in-game or via console using `/mod <verb> [part of mod name]` (skip the `/` in console).  

Verbs are:

* `list|l`
* `enable|e [name]`
* `disable|d [name]`
* `restart|r [name]`
* `toggle|t [name]`


Three samples are included which split the functionality of the template:

* `CritOverride` loads/creates `Settings.json` in its directory when it starts.  It uses the value in that and a Harmony attribute to overriding weapon crit chance.
* `KillCount` tracks kill count by-player-by-monster and lets you set an XP multipler that will trigger every *n*th kill.
* `Discord` is a rework of [Discord Relay](https://github.com/aquafir/ACE/wiki/Discord-Relay).  
  It requires [creating a bot](https://github.com/aquafir/ACE/wiki/Discord-Relay#your-bot) and supplying a channel ID and bot token to `Settings.json`.  
  You also need to add [Discord.Net.Websocket](https://www.nuget.org/packages/Discord.Net.WebSocket) *to the server*.  Probably better ways of doing this.
  Currently it isn't relaying messages from Discord to ACE.  ACE to Discord works.
* `CleaveTranspiler` is an example of using Harmony transpilers to directly change CIL to modify the angle, range, and max targets for cleaving.  [See here](https://github.com/aquafir/ACE.BaseMod/tree/master/CleaveTranspiler) for details.
* `Spells` adds three things with a few options:
  * A pool of spell IDs that trigger on UA attack if the right combination of height/power is used.
    * Control number of buckets slider is divided into.
    * The pool defaults to rings.
  * A static remapping of spells in landblocks that *have a dungeon* that uses the landblock ID to shift a spell to another in its group.
  * A random remapping each cast to another in its group.
  * The above two can be limited to only the `PlayerSpellTable`, to replace before/after a cast (e.g., change animation), and to create groups that are more loosely related.





### Todo

* Look at using [Reverse patches](https://harmony.pardeike.net/articles/reverse-patching.html) to snapshot/restore any patched methods.
* Good example for using [Finalizer patches](https://harmony.pardeike.net/articles/patching-finalizer.html) to eat tricky and inconsequential exceptions.
* Exposing capabilities of mods to other mods
  * `ModContainer`-based dependencies / incompatibilities.
* Threading
