This adds areas that are exempt from a connection limit to allow additional connections for utility purposes (e.g., bots, muling).

*  `MaxNonExempt` sets a limit for players outside of exempt landblocks.  
  Increase the number of allowed ACE connections and use this setting for the number of "active" connections.
* `ExemptIPAddresses` sets the allowed connections per IP address. This is an object with the keys being IP addresses and the value being an integer of the number of allowed connections.
* `ExemptLandblocks` is a set of [landblocks](https://docs.google.com/spreadsheets/d/122xOw3IKCezaTDjC_hggWSVzYJ_9M_zUUtGEXkwNXfs/edit#gid=734303881) that will be exempt from this limit.
* `Interval` is the number of seconds between checks for players that exceed the limit.  



Another approach that checks on portal exit was implemented and left disabled in `PatchClass`.
