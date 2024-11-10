

## Quality of Life

Combined mod of some basic convenience features.



Each patch category must be present in `Patches` to be enabled, otherwise they will make no changes to ACE:

```
  "Patches": [
    "animations",
    "defaults",
    "fellowship",
    "recklessness",
    "tailoring"
  ],
```



### Fellowship

* `MaxMembers` sets max fellowship members
* `SharePercent` is a dictionary of the number of members and the share percent
* `DefaultShare` is what is used if a specific share percent is not found
* ~~`EvenShareLevel` sets the level at which experience is shared evenly~~ 
  * Removed to respect non-default settings.  All it does is set/restore a server property

* `SendDetails` displays relevant information to players when recruiting.



The `/fship` command is added:

* `/fship`  invites all in the current landblock
* `/fship [part of name]` invites all online matching the parameter



### Recklessness

* `PowerLow` / `PowerHigh` set the min/max ranges for using Recklessness.
* `RatingTrained` / `RatingSpecialized` set the bonuses to rating.



### Animations

Whenever an [animation](https://github.com/ACEmulator/ACE/blob/fdfdec9f0a16bbcbb89a9120ce4f889520a51708/Source/ACE.Entity/Enum/MotionCommand.cs#L7) is set to something from a `null` value, if a default is in `AnimationSpeeds` it will instead be set to that.

* Intercepts `MotionTable.GetAnimationLength`

* Need to think about `MotionStance` and things not grabbing length, like `/mp`
* Many other things like `MotionTable.GetAttackFrames` not covered



##### Workarounds

* `DieSeconds` sets seconds between progressing the next stage of the `/die` command





### Augmentations

* `IgnoreSharedAttribute` and `IgnoreSharedResist` ignored group total restrictions for augmenting attributes or resists.
* `MaxAugs` overrides any existing max augmentation value.



### Property Defaults

Whenever a property is set to something from a `null` value, if a default is in the corresponding property dictionary it will instead be set to that.



### Credits

* `MaxSpecCredits` sets the number of specialization credits
  * Fakes having more spec credits by `70 - max`
  * Rewrite of `SkillAlterationDevice` might work?



### Tailoring

Enables the use of Society Armor for tailoring.



### RunAs

Experimental command that lets you run a command as an online player without regular checks:

* `runas <player> <command with params>`
* e.g., `runas alttest faction ch 5`
