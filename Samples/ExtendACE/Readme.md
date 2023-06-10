Attempt to extend enums of ACE.



`CreatureEx` extends `Creature` with different subclasses overriding or patching for singular purposes.  It includes a patch for the `WorldObjectFactory.CreateWorldObject` methods to intercept new creatures and replace them with a variant from the available `CreatureType`.

You can create them using the `/cex [type]` command:

* Accurate has a flat chance of hitting
* //Avenger
* //Bard
* Berserker will increase all attributes once on low health, proportional to remaining health
* Boss
  * Increases attributes, size, and xp
  * Cannot be resisted or evaded
  * Rotating weakness.  Everything else deals reduced damaged
  * Periodically fires volleys of select spells at all nearby players
  * Periodically sends many slow, spell projectiles from random directions at the `AttackTarget`
* Comboer will increase a counter on successful hit and reset when it gets hit.  When the counter exceeds 5/10 it will cast a ring/ring II
* Drainer attacks mana or stamina instead of health
* Duelist avoids damage if they are facing the player
* Evader has a flat chance of avoiding an attack
* Exploder ticks down when near a player, exploding for percent of health damage
* *Healer heals nearby creatures*
* Merger periodically tries to merge with nearby creatures of the same type, increasing hp, xp, and scaling stats.
* //Necromancer
* //Poisoner
* Puppeteer spawns unattackable copies of itself which all die when it dies.  *Need to look into generator / location handling.*
* //Reaper
* Rogue disarms equipment if attacking from behind
* //Runner
* Shielded neutralizes damage at the cost of a shield, which periodically replenish
* SpellBreaker cancels casting and deals damage from the cancelled spell
* SpellThief steals a random enchantment
* //Splitter
* Stomper damages in a radius for a fraction of damage dealt (post mitigation) based on how far you are from the center
* *Stunner periodically stuns a nearby player.  This cancels casts/combat and plays the kneel animation. Doesn't prevent restarting cast/combat while kneeling.*
* //Suppresser
* Tank intercepts damage from something nearby at a reduced rate
* Vampire heals from damage dealt
* Warder prevents targeting other nearby non-Warder creatures with spells





## Notes

* Property enums are ushort
* `Creature.OnDamageTarget` is a placeholder for Player that does nothing.  Never called.
