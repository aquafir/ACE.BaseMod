* SSF players are unable to equip any item they don't have a claim to
  * A `PropertyInstanceId` set by `OriginalFinder` is used to track the GUID of a claim on the item
* Chests that are unlocked that don't have a claimant are claimed by the unlocker
  * Claimed status reset on chest reset
* Corpses are claimed by killer
* Hardcore
  * Creature
    * Die
    * CreateCorpse



* Todo?
  * Retrict by dungeon 
  * Indicator of SSF items?
    * Overlay/color
    * GetLongDesc
      * Player's X
      * `GetLongDesc`
    * AppraiseInfo.BuildProfile
    * PropertiesString
  * Support non-SSF players?
    * Non SSF players have no restrictions
  * Restrictions beyond equip?
    * Using
    * Selling
    * Giving
    * Picking up

  * LootFactory.CreateRandomLootObjects
    * Used by corpses / treasure generation
    * Make it trigger on open?
  





* QoL
  * die / recall speed