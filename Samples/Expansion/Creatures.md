## CreatureEx

***Disclaimer: Very little testing has been done with the various types***



`CreatureEx` extends `Creature` with different subclasses overriding or patching for singular purposes.  It includes a patch for the `WorldObjectFactory.CreateWorldObject` methods to intercept new creatures and replace them with a variant from the available [CreatureExType](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/CreatureExType.cs).



You can create them using the `/cex [type]` command, with a random 0-1 chance set by `CreatureChance`, or adding a `CreatureExType` [FakeInt](https://github.com/aquafir/ACE.BaseMod/blob/master/ACE.Shared/FakeProperty.cs) (10029 at present) to a weenie with the value being the [CreatureExType](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/CreatureExType.cs).  This requires the `CreatureEx` `Feature` to be enabled.



If there are accompanying Harmony patches that are required the `CreatureExType` (used for the patch category) must be present in `CreatureFeatures`.



### Types

* [Accurate](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Accurate.cs) has a flat chance of hitting
* //[Avenger](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Avenger.cs)
* //[Bard](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Bard.cs)
* [Banisher](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Banisher.cs) destroys summoned pets of the `AttackTarget`
* [Berserker](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Berserker.cs) will increase all attributes once on low health, proportional to remaining health
* [Boss](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Boss.cs)
  * Increases attributes, size, and xp
  * Cannot be resisted or evaded
  * Rotating weakness.  Everything else deals reduced damaged
  * Periodically fires volleys of select spells at all nearby players
  * Periodically sends many slow, spell projectiles from random directions at the `AttackTarget`
* [Comboer](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Comboer.cs) will increase a counter on successful hit and reset when it gets hit.  When the counter exceeds 5/10 it will cast a ring/ring II
* [Drainer](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Drainer.cs) attacks mana or stamina instead of health
* [Duelist](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Duelist.cs) avoids damage if they are facing the player
* [Evader](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Evader.cs) has a flat chance of avoiding an attack
* [Exploder](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Exploder.cs) ticks down when near a player, exploding for percent of health damage
* *[Healer](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Healer.cs) heals nearby creatures*
* [Merger](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Merger.cs) periodically tries to merge with nearby creatures of the same type, increasing hp, xp, and scaling stats.
* //[Necromancer](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Necromancer.cs)
* //[Poisoner](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Poisoner.cs)
* [Puppeteer](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Puppeteer.cs) spawns unattackable copies of itself which all die when it dies.  *Need to look into generator / location handling.*
* //[Reaper](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Reaper.cs)
* [Rogue](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Rogue.cs) disarms equipment if attacking from behind
* //[Runner](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Runner.cs)
* [Shielded](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Shielded.cs) neutralizes damage at the cost of a shield, which periodically replenish
* [SpellBreaker](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/SpellBreaker.cs) cancels casting and deals damage from the cancelled spell
* [SpellThief](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/SpellThief.cs) steals a random enchantment
* //[Splitter](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Splitter.cs)
* [Stomper](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Stomper.cs) damages in a radius for a fraction of damage dealt (post mitigation) based on how far you are from the center
* *[Stunner](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Stunner.cs) periodically stuns a nearby player.  This cancels casts/combat and plays the kneel animation. Doesn't prevent restarting cast/combat while kneeling.*
  * Status effects a queue/HashSet in a manager?  Part of CreatureEx?  Through enchantments?

* //[Suppresser](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Suppresser.cs)
* [Tank](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Tank.cs) intercepts damage from something nearby at a reduced rate
* [Vampire](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Vampire.cs) heals from damage dealt
* [Warder](https://github.com/aquafir/ACE.BaseMod/blob/master/Samples/Expansion/Creatures/Warder.cs) prevents targeting other nearby non-Warder creatures with spells


### Videos

Example of spawning Tank and Drainer

https://github.com/aquafir/ACE.BaseMod/assets/83029060/eb2be86e-b35d-4708-8450-38f24e5582d7
