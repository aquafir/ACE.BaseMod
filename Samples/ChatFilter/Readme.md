* Uses [Profanity.Detector](https://github.com/stephenhaunts/ProfanityDetector)
* If `UseDefaultList` is enabled it uses the default list
  * Customize with `blacklist.txt` and `whitelist.txt`
  * [Additional list](https://github.com/surge-ai/profanity) available (thanks Maethor)
* Set `FilterChat` / `FilterTells` to `true` to filter the corresponding communication or `false` to ignore
* If `ShadowBan` is enabled and a player has/will-be chat banned they will see their message appear but it won't be sent.
* If `GagPlayer` is enabled it will gag the player for `GagBaseTime` seconds plus `GagTimePerInfraction` per infraction.
  * If `BroadcastGag` is enabled it will broadcast the gag.
* If `BanPlayer` is enabled it will ban the player for `BanBaseTime` seconds plus `BanTimePerInfraction` per infraction.
  * If `BroadcastBan` is enabled it will broadcast the ban.
* If `CensorText` is true the message will replace matches with `*`