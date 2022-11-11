**Respawn** adds three main commands:

* `/left` lets a `Player` see the current/max creatures in a landblock.  If `DetailedDump` is enabled in the settings it also breaks that down by creature group.
* `/spawn [amount]` lets an `Admin` respawn a landblock to full (if `ExceedMax` is disabled), with a complete spawn (if enabled), or up to `amount`.
* `/smote [amount]` lets an `Admin` smite everything in a landblock, or up to `amount`.

A number of extension methods are added to `Landblock` to assist in getting the current and max number of creatures.  Accessing private fields is done via the `Traverse` [utility class](https://harmony.pardeike.net/articles/utilities.html).

Using those helpers, every `Interval` seconds each Dungeon landblock is checked for the percent alive out of the max creatures the static generators in that landblock can create.

If that percentage dips below the `RespawnTreshold` it will cause the the landblock to respawn similar to the `spawn` command.

If `RewardLastKill` is enabled the mod will also manually patch a method to keep a record of the player with the most recent kill in each landblock.  When a respawn is triggered, the player will be given `RewardAmount`xp.  If `SpamPlayer` is on they will be informed after every kill of the number remaining.

There's still probably some bugs and things to decide about cacheing or how to handle other landblock types.

[Demo video](https://www.youtube.com/watch?v=rFaNTf0FbgE)


