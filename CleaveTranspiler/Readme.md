#  CleaveTranspiler

`CleaveTranspiler` is an example of using [Harmony transpilers](https://harmony.pardeike.net/articles/patching-transpiler.html) to directly change CIL to modify the angle and range of cleaving.

It also sets the number of cleave targets by changing the `Getter` of `WorldObject.CleaveNumber` and patches `WorldObject.IsCleaving` to enable cleave for all melee weapons.

---

`GetCleaveTarget` uses some readonly fields to check for valid targets by angle and distance:

```c#
    public static readonly float CleaveAngle = 180.0f;
    public static readonly float CleaveCylRange = 2.0f;
```

Harmony isn't able to patch fields, so a different approach is needed to change those.

One option would be to use a [prefix](https://harmony.pardeike.net/articles/patching-prefix.html) that replaces the functionality and skips the original method.  This would be done by copying the code of the method and anything it depends on and changing it as desired.

Another solution is to use a [transpiler](https://harmony.pardeike.net/articles/patching-transpiler.html) patch to modify the instructions of the `GetCleaveTarget` method directly.

With [dnSpy](https://github.com/dnSpyEx/dnSpy) (optional [GoToDnSpy](https://marketplace.visualstudio.com/items?itemName=VladimirChirikov.GoToDnSpy) VS extension) or [ILSpy](https://github.com/icsharpcode/ILSpy#ilspy-------) ([extension](https://marketplace.visualstudio.com/items?itemName=SharpDevelopTeam.ILSpy)) you can look at the [instructions](https://en.wikipedia.org/wiki/List_of_CIL_instructions) that are created by the C# code to find what you want to change.  This mod wants to replace usage of the fields above, so it starts by using `AccessTools` to get a reference to their `FieldInfo`:

```c#
        static FieldInfo f_cleaveAngle = AccessTools.Field(typeof(Creature), nameof(Creature.CleaveAngle));
        static FieldInfo f_cleaveCylDistance = AccessTools.Field(typeof(Creature), nameof(Creature.CleaveCylRange));
```

In the transpiler patch the `CodeInstruction`'s are looped through.  If an instruction is found that uses the `CleaveAngle` or `CleaveCylRange` field it is replaced with an instruction to load a float32 value from those provided in `Settings.json`:

```c#
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Creature), nameof(Creature.GetCleaveTarget), new Type[] { typeof(Creature), typeof(WorldObject) })]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].LoadsField(f_cleaveAngle))
                    codes[i] = new CodeInstruction(OpCodes.Ldc_R4, _settings.CleaveAngle);
                
                if(codes[i].LoadsField(f_cleaveCylDistance))
                	codes[i] = new CodeInstruction(OpCodes.Ldc_R4, _settings.CleaveCylRange);
            }

            return codes.AsEnumerable();
        }
```