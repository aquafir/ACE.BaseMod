## EasyEnlightenment

Adds flexibility to enlightenments.

* Set a bonus per Enlightenment in Dictionaries 
  * For `PropertyFloat|Int`
    * `IntAugments` 
    * `FloatAugments`

  * Bonuses that require `Expansion` with the `BonusStats` Feature enabled
    * `SkillAugments`
    * `AttributeAugments`
    * `VitalAugments`

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



Commands:

* `/newlum` command allows a conversion from a previous system to a modified one.  Players will be flagged as being on the new system after conversion.
* `/fixee` reapplies the correct bonuses, equal to the bonus per Enlightenment times number of Enlightenments

