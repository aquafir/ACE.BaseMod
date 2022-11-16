# Spells

`Spells` adds four things with a few options:

- WIP implementation of `/meta [scale]` that creates per-person per-spell copies of a `Spell` and `SpellBase`
- Settings to control splitting projectile or splashing debuffs.
  - Set number of targets .
  - Set distance of the original target to look for targets.
  - Set the number of seconds between triggers.
  - Should perform as well as cleave.  
  *It uses the casting player's visible objects as a reference, so there may be a situation where the target is far enough away that it misses targets visible to it but not the player.  An  option would check distance from reference player to target, and fall back to using something like the landblock's creature list.*
- A pool of spell IDs that trigger on UA attack if the right combination of height/power is used.
  - Control number of buckets slider is divided into.
  - The pool defaults to rings.
- A static remapping of spells in landblocks that *have a dungeon* that uses the landblock ID to shift a spell to another in its group.
  - Adds a learning curve for the landblock.
- A random remapping each cast to another in its group.



For the last two, some options are:

* Limit spells to only the `PlayerSpellTable`, 
* Replace before or after a cast (e.g., change animation)  
* Create groups that are more or less similar
  * **C**omparable would be Fire 1-->Fire 1 to 8
  * **R**elated would be Fire 1-->Any War with similar targets
  
  
### Examples

<details><summary>Meta Spells</summary>
<video src="https://user-images.githubusercontent.com/83029060/202056553-bad9fd90-f169-40c6-802b-87f991f1eb67.mp4"></video>
</details>



<details><summary>Splash and Split</summary>
<video src="https://user-images.githubusercontent.com/83029060/201587184-88a86dd8-eef2-4804-a494-e7920dba14e8.mp4"></video>
</details>



<details><summary>Fist Sweetspots</summary>
<video src="https://user-images.githubusercontent.com/83029060/200747153-65b42854-d1a8-4d57-a79f-7289b3eb30a4.mp4"></video>
</details>



<details><summary>Remapping Consistently</summary>
<video src="https://user-images.githubusercontent.com/83029060/200746579-5069413b-3620-43c6-a089-50e75050feaa.mp4"></video>
</details>



<details><summary>Random Before/After Animation</summary>
<video src="https://user-images.githubusercontent.com/83029060/200746717-b2f87fb0-e597-4f01-a3ba-e8777634a7a4.mp4"></video>
</details>

