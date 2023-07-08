Still a work in progress.

Adds: 
* `/save <character name>[, save name]` -- defaults to the name of the character being saved with a date/time stamp.
* `/load <part of save name>, [account name|ID,] <new character name>` -- defaults to using the account of the loaded character if missing.


### Todo

* ~~BinaryReader / BinaryWriter~~
  * Decide on clearing collections, vs. adding possible duplicates, vs. adding duplicates and removing non-unique after
* Decide on House and Allegiance
* Support for cross-server moves
  * Add server-of-origin to saves
  * Add map of IDs to player name for all player-related properties
    * Search for player names on new server and add to ID swaps
* Export/import accounts+
* Find player names with IDs and store in save
* Send file to player
