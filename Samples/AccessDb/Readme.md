This mod has a few basic examples of working with the online/offline player lists and database.

* `moveall` uses the online/offline lists to either teleport or set the login location for all players to a location (e.g., `moveall 10s 20e`)

* `plocs` uses `PlayerManager.GetAllOffline()` to display the current and sanctuary location of all players.

* `ctypes` uses `WorldDbContext` to display creature types and the number of types of creatures belonging to that type

* `lcount` uses `ShardDbContext` to display the number of logins for each player / `ShardDbContext`

  