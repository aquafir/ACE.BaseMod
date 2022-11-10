*Thanks to Jkurs for the original `/raise`*

This is a ported version of a refactor of `/raise` and `/raiserefund`.

Instead of keeping track of a raised `RaiseTarget` in the database through an ACE change this keeps track in `History.json` which is saved when the mod is disabled.

Note that changing the settings will cause an over/underestimate of refunded costs.  An improvement would allow a server-wide refund or tracking costs separately.

https://user-images.githubusercontent.com/83029060/201008467-63116461-0b20-4b09-a174-4d2c08eeedfe.mp4

