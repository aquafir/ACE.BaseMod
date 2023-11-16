* When you log in it calculates your quest bonus
  * Using a custom value if specified in `QuestBonuses`
  * Or a default value if missing, from `DefaultPoints`
  * Storing it as a `PropertyFloat` specified by `QuestBonusProperty`
* Finishing a quest updates the bonus, notifying the player of the change if `NotifyQuest` is true
* Gained experience is multiplied it by your bonus
  * 1+(`BonusConversion` * quest points)
  * Player is notified of the boost if `NotifyExp` is true