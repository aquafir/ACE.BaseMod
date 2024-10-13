

### Augments

An `Augment` describes a reversible, scalable change to a `WorldObject`:

* `TryAugment(this WorldObject wo, AugmentType type, int key, Operation op, double value)`
* `AugmentType`
  * Determines what sort of property is changed
  * Has an int `key` to determine which specific property of a category is changed
    * The value is gotten with `TryGetAugmentTargetValue(this WorldObject wo, AugmentType type, int key, out object? value)` 
  * Related to `StatType` of Recipes
  * Can be anything but a `PropertyString` amongst properties
    * *Todo: Skill / SAC / player-related properties*
* [`Operation`](#Operation) and `value`  are used to try to change the target property







* Get the value
  * If missing, short circuit depending on the operation.  Otherwise assume 0?
  * Some AugmentTypes like vitals require the `WorldObject` be something like a `Creature`
* Apply the operation to the values
* Convert the value to 



### Affixes 

An `Affix` is a collection of `Augment` which may apply to some eligible WorldObject, [randomly selected](https://github.com/cdanek/KaimiraWeightedList) based on its weight.







* https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp
* https://github.com/BlueRaja/Weighted-Item-Randomizer-for-C-Sharp/wiki/Getting-Started#are-floating-point-weights-supported
* https://github.com/cdanek/KaimiraWeightedList
* https://www.keithschwarz.com/darts-dice-coins/





### Operation

* Determines how the property is changed
* Related to `MutationEffectType`



## Cases

* Missing values
  * Missing X to be divided/multiplied by 5
    * Fail
  * Missing X to have 5 added
    * 5
  * M







## Reference

* `double` can store up to `2^53` , 1024 times less than `long.MaxValue`



Possible Operations:

### **Unary Operations**

- **Negation**: `-x`
- **Bitwise NOT**: `~x`
- **Increment**: `++x`, `x++`
- **Decrement**: `--x`, `x--`

### **Binary Arithmetic Operations**

- **Addition**: `x + y`
- **Subtraction**: `x - y`
- **Multiplication**: `x * y`
- **Division**: `x / y`
- **Modulus**: `x % y`

### **Bitwise Operations**

- **AND**: `x & y`
- **OR**: `x | y`
- **XOR**: `x ^ y`
- **Left Shift**: `x << n`
- **Right Shift**: `x >> n`

### **Assignment Operations**

- **Assignment**: `x = y`
- **Compound Assignment**: `x += y`, `x -= y`, `x *= y`, etc.

### **Comparison Operations**

- **Equality**: `x == y`
- **Inequality**: `x != y`
- **Greater Than**: `x > y`
- **Less Than**: `x < y`
- **Greater or Equal**: `x >= y`
- **Less or Equal**: `x <= y`

### **Logical Operations**

- **Logical AND**: `&&`
- **Logical OR**: `||`
- **Logical NOT**: `!x`
