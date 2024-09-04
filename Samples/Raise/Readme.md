## Raise

This mod adds common "infinite" features like a custom max level and `/raise` command:

* `/raise` can be used on any attribute and a few `PropertyInt` values that correspond to Luminance ratings.  Adding more is easy.
* `/raiserefund` (`/rr`) refunds the player and invested resource
* `/raiselist` (`/rl`) sends the player a list of their raised levels and resources spent

The number of times a target has been raised is stored using an offset to its enum value as a `PropertyInt`.


https://github.com/aquafir/ACE.BaseMod/assets/83029060/4b79d8e0-cc86-4359-802d-c15d5ee79497





`LevelCost` are used to define the function that determine costs:

* `C` is a constant

* `Rate` = `r`, the rate of growth 

* `Coefficient` = `a`, scales the polynomial term

* `Offset` is added to the level, `n`

  * Such as if you want to ignore all the levels coming from the standard costs

* Available `GrowthType` are:

  * `Linear` 
    * 
    * $C(n)=C+n*r$

  * `Exponential`
    * Use if you want each level to cost a fraction more than the previous
    * $C(n)=C*r^n$
  * `Polynomial`
    * $C(n)=C+a*n^r$
