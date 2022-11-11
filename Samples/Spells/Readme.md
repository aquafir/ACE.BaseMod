# Spells

`Spells` adds four things with a few options:

- A pool of spell IDs that trigger on UA attack if the right combination of height/power is used.
  - Control number of buckets slider is divided into.
  - The pool defaults to rings.
- A static remapping of spells in landblocks that *have a dungeon* that uses the landblock ID to shift a spell to another in its group.
  - Adds a learning curve for the landblock.
- A random remapping each cast to another in its group.
- Settings to control spells splitting multiple targets sorted by closeness to the original target.



For the last two, some options are:

* Limit spells to only the `PlayerSpellTable`, 
* Replace before or after a cast (e.g., change animation)  
* Create groups that are more or less similar
  * **C**omparable would be Fire 1-->Fire 1 to 8
  * **R**elated would be Fire 1-->Any War with similar targets
  
  
### Examples

#### Fist Sweetspots

https://user-images.githubusercontent.com/83029060/200747153-65b42854-d1a8-4d57-a79f-7289b3eb30a4.mp4



#### Remapping Consistently

https://user-images.githubusercontent.com/83029060/200746579-5069413b-3620-43c6-a089-50e75050feaa.mp4



#### Random Before/After Animation

https://user-images.githubusercontent.com/83029060/200746717-b2f87fb0-e597-4f01-a3ba-e8777634a7a4.mp4

