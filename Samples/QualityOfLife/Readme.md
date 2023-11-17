﻿## QoL

* `MaxSpecCredits` sets the number of specialization credits
  * Fakes having more spec credits by `70 - max`
  * Rewrite of SkillAlterationDevice might work?



## Animations

If `OverrideAnimations`  is set, whenever an [animation](https://github.com/ACEmulator/ACE/blob/fdfdec9f0a16bbcbb89a9120ce4f889520a51708/Source/ACE.Entity/Enum/MotionCommand.cs#L7) is set to something from a `null` value, if a default is in `AnimationSpeeds` it will instead be set to that.

* Intercepts `MotionTable.GetAnimationLength`

* Need to think about `MotionStance` and things not grabbing length, like `/mp`
* Many other things like `MotionTable.GetAttackFrames` not covered



###### Workarounds

* `DieSeconds` sets seconds between progressing the next stage of the `/die` command



## Property Defaults

If `OverrideDefaultProperties` is set, whenever a property is set to something from a `null` value, if a default is in the corresponding property dictionary it will instead be set to that.
