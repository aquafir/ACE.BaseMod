This reimplements/overrides `RecipeManager.VerifyRequirements` 

* When a `PropertyInt.NumTimesTinkered` is the requirement it uses `MaxTries` instead of the default.
* `MaxTries` is also used to add difficulty entries growing by `Scale` 







## Imbues

* ImbuedEffect2-5 don't seem to be used
* |= to add an imbue to normal
* `Recipe.GetTinkerChance` for 1/3 chance
