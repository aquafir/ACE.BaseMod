# Spells

`Spells` adds three things with a few options:

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