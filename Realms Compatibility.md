Initial [fork of BaseMod](https://github.com/ACRealms/ACE.BaseMod) by RF referenced for how to convert stock ACE code to [ACRealms](https://github.com/ACRealms/ACRealms.WorldServer) for support of instancing and other features.

## Conversions

* `CommandHandler` signatures
  * `public static void MyCommand(Session session, params string[] parameters)` becomes
  * `public static void MyCommand(ISession session, params string[] parameters)`
  * Replace/with
    * \]\s*\r?\n\s*(public static void \w+)(\(Session session, params string\[\] parameters\))
      * End of command attribute followed by a method without conditions
    * ]\r\n#if REALM\r\n$1(ISession session, params string[] parameters)\r\n#else\r\n$1$2\r\n#endif
  * Unrelated find/replace to add command categories to Features
    * ^.*(nameof\(Feature.\w+\)).*
    * [CommandCategory($1)]\r\n[HarmonyPatchCategory($1)]\r\n

* `IID`
  * Change from uint to ulong
* `NetworkManager`
  * Add `.Instance`
* `Creature` ?
  * Has an added `AppliedRuleset` in constructor
    * public Accurate(Weenie weenie, ObjectGuid guid, **AppliedRuleset ruleset**) : base(weenie, guid, **ruleset**) { }
    * Replace/with
      * ^(.*)\(Weenie weenie, ObjectGuid guid\) : base\(weenie, guid\)
      * #if REALM\r\n$1(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)\r\n#else\r\n$1(Weenie weenie, ObjectGuid guid) : base(weenie, guid)\r\n#endif



### Positions

* `Position` to `LocalPosition`
  * var portalDest = new LocalPosition(weenie.GetPosition(PositionType.Destination));
    var portalDestLocal = new LocalPosition(weenie.GetPosition(PositionType.Destination));
    var portalDest = portalDestLocal.AsInstancedPosition(Position.InstanceIDFromVars((ushort)ReservedRealm.@default, 1, false));
  * `.AsLocalPosition()`
  * 
* `GetPosition`
* `ACEPosition` requires instance uint instance of an `InstancedPosition`
  * Find/replace (probably)
    * ^(.*ACEPosition\()(\).*)$
    * #if REALM\r\n$1__instance.Location.Instance$2#else\r\n$1$2#endif\r\n

