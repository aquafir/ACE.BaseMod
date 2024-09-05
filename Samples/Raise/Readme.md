## Raise

This mod lets you define a `MaxLevel` and offers things to do with that excess experience.

* Skill credits are given every `CreditInterval` levels beyond the last standard one
* Each level increases at a rate determined by `LevelCost`



`LevelCost` are used to define the function that determine costs:

* `C` is a constant

* `Rate` = `r`, the rate of growth 

* `Coefficient` = `a`, scales the polynomial term

* `Offset` is added to the level, `n`

  * Such as if you want to ignore all the levels coming from the standard costs

* Available `GrowthType` are:

  * `Linear`
    * Use if you want each level to cost a constant `Rate` more than the previous
    * $F(n)=C+n*r$

  * `Exponential`
    * Use if you want each level to cost the previous mulitplied by `Rate`
    * $F(n)=C*r^n$
  * `Polynomial`
    * $F(n)=C+a*n^r$





## Alternate Levels

Alternate levels replace how ACE handles spending experience on skills, attributes, and vitals using corresponding `LevelCost`:

* `Attribute`
* `Vital`
* `Trained`
* `Specialized`



`LevelPropertyStart` and `SpendPropertyStart` determine what PropertyInt and PropertyInt64 store the alternate levels and the total amount spent on them.



`PreferStandard` uses standard ACE values while available.

If using standard levels, `OffsetByLastStandard`  treat the first non-standard level as the first.



## Infinite

`Raise` was a quick take (should be reworked) on adding common "infinite" features:

* `/raise` can be used on any attribute and a few `PropertyInt` values that correspond to Luminance ratings.
* `/raiserefund` (`/rr`) refunds the player and invested resource
* `/raiselist` (`/rl`) sends the player a list of their raised levels and resources spent

The number of times a target has been raised is stored using an offset to its enum value as a `PropertyInt`.


https://github.com/aquafir/ACE.BaseMod/assets/83029060/4b79d8e0-cc86-4359-802d-c15d5ee79497





