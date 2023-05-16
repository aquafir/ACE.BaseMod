Gives control over a variety of balance-related features.



This uses [AngouriMath](https://am.angouri.org/docs/AngouriMath/MathS.html) to redefine performant formulas for different things in ACE.

* [Docs](https://am.angouri.org/docs/namespaces.html)
* String syntax
  * [Sets](https://am.angouri.org/docs/AngouriMath.Entity.Set.html)
    * {1, 2, 3}
    * /\
    * \/
    * `5 in ([1; +oo) \/ { x : x < -4 })`
* [CompilationProtocol](https://habr.com/en/articles/546926/)





## Formulas

* *Setting*
  * *Formula*
    * *Enums for piecewise functions if applicable.  Starting at 0 unless otherwise specified* 
  * *Default*
  * 



###### Experience

* ExperienceFormula

  * x = Xp amount, t = type, s = share type, n = number of active connections

    * t = `Kill, Quest, Proficiency, Fellowship, Allegiance, Admin, Emote`

  * 

    



###### Levels

* CostPerLevelFormula
  * x = level
* MaxLevel
  * No formula, but if desired there could be a per-Player max level
* [Math.Net Symbolics](https://github.com/mathnet/mathnet-symbolics) was used to extrapolate costs based on existing ones if a formula isn't provided. 



###### Nether



Set max level and cost-per-level.

* Interpolate level past 275+ using the original curve.





