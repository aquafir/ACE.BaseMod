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
