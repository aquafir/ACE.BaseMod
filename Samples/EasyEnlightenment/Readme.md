## EasyEnlightenment

Adds flexibility to enlightenments.

* `IntAugments` / `FloatAugments`
  * These augment a player with a set value of any number of `PropertyFloat|Int`
* `MaxEnlightenments` 
* `LevelReq`  / `RequireSocietyMaster` / `RequireAllLuminanceAuras` set corresponding requirements
* `SkillCreditAmount` / `SkillCreditInterval` are used to add an amount of credits based on number of enlightenments
* `SkipNormalBroadcast` 
* `SkipResetCertificate` prevents item creation
* `MaxLumBase` / `MaxLumPerEnlightenment`

Change what is removed on enlightenment with:

* `RemoveSociety`
* `RemoveLuminance`
* `RemoveAetheria`
* `RemoveAttributes`
* `RemoveSkills`
* `RemoveLevel`

The `newlum` command allows a conversion from a previous system to a modified one.  Players will be flagged as being on the new system after conversion.

