

### Affixes and Augments

An `Augment` describes a reversible change to a `WorldObject`:

* `TryAugment(this WorldObject wo, AugmentType type, AugmentOperation op, int key, double value, bool recordAugment = true)`
* `AugmentType`
  * Determines what sort of property is changed
  * Has an int `key` to determine which specific property of a category is changed
  * Related to `StatType` of Recipes
  * Can be anything but a `PropertyString` amongst properties
    * *Todo: Skill / SAC / player-related properties*

* `Operation`
  * Determines how the property is changed
  * Related to `MutationEffectType`
  * Operations are:
    * Assign
    * Add
    * Multiply
    * BitSet
    * BitClear

* `Value`
  * Normalized value the target property is set to
* When applied it 





An `Affix` is a collection of `Augment` which may apply to some eligible WorldObject, [randomly selected](https://github.com/cdanek/KaimiraWeightedList) based on its weight.







* https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp
* https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started#are-floating-point-weights-supported
* https://github.com/cdanek/KaimiraWeightedList
* https://www.keithschwarz.com/darts-dice-coins/





Questions for RF

* Performance of `ActionChain`
* How to handle mutations of stackable items
