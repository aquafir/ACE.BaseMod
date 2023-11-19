## ChatFilter

* Uses [Profanity.Detector](https://github.com/stephenhaunts/ProfanityDetector)
* If `UseDefaultList` is enabled it uses the default list
  * Customize with `blacklist.txt` and `whitelist.txt`
  * [Additional list](https://github.com/surge-ai/profanity) available (thanks Maethor)
* Set `FilterChat` / `FilterTells` to `true` to filter the corresponding communication or `false` to ignore
* If `ShadowBan` is enabled and a player has/will-be chat banned they will see their message appear but it won't be sent.
  * They will not be gagged or banned.
  * The `/unsban [name]` command can be used to remove a shadow ban
* If `GagPlayer` is enabled it will gag the player for `GagBaseTime` seconds plus `GagTimePerInfraction` per infraction.
  * If `BroadcastGag` is enabled it will broadcast the gag.
* If `BanPlayer` is enabled it will ban the player for `BanBaseTime` seconds plus `BanTimePerInfraction` per infraction.
  * If `BroadcastBan` is enabled it will broadcast the ban.
* If `CensorText` is true the message will replace matches with `*`


### Filter

https://github.com/aquafir/ACE.BaseMod/assets/83029060/e9c406b3-2cd5-4e1e-a2d8-a3d2e8f53dc1

### Shadow Ban

https://github.com/aquafir/ACE.BaseMod/assets/83029060/b71925a5-55cf-4a15-9d02-cc775159a892

