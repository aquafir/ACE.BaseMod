namespace ACE.Shared.Helpers;

public static class SpellExtensions
{
    public static void SetCloakSpellProc(this WorldObject wo, SpellId spellId)
    {
        if (spellId != SpellId.Undef)
        {
            wo.ProcSpell = (uint)spellId;
            wo.ProcSpellSelfTargeted = spellId.IsSelfTargeting();
            wo.CloakWeaveProc = 1;
        }
        else
        {
            // Damage Reduction proc?
            wo.CloakWeaveProc = 2;
        }
    }

    //Todo: decide whether I need to create an instance of the spell to check?
    //CloakAllId was the original cloak check
    //Aetheria uses a lookup
    public static bool IsSelfTargeting(this SpellId spellId) => new Spell(spellId).IsSelfTargeted; //spellId == SpellId.CloakAllSkill;

    #region Spells / SpellBase Code
    

    //public static SpellBase DeepClone(this SpellBase spellBase)
    //{
    //    //return spellBase.CloneJson<SpellBase>();
    //    //SpellBase clone = ByteArrayToObject(ObjectToByteArray(spellBase)) as SpellBase;

    //    var clone = new SpellBase();

    //    MemoryStream ms = new();
    //    BinaryWriter writer = new(ms);
    //    BinaryReader reader = new(ms);

    //    writer.WriteObfuscatedString(spellBase.Name);
    //    writer.WriteObfuscatedString(spellBase.Desc);

    //    writer.Write((uint)spellBase.School);
    //    writer.Write(spellBase.Icon);
    //    writer.Write((uint)spellBase.Category);
    //    writer.Write(spellBase.Bitfield);
    //    writer.Write(spellBase.BaseMana);
    //    writer.Write((float)spellBase.BaseRangeConstant);
    //    writer.Write((float)spellBase.BaseRangeMod);
    //    writer.Write(spellBase.Power);
    //    writer.Write((float)spellBase.SpellEconomyMod);
    //    writer.Write(spellBase.FormulaVersion);
    //    writer.Write((float)spellBase.ComponentLoss);
    //    writer.Write((uint)spellBase.MetaSpellType);
    //    writer.Write(spellBase.MetaSpellId);

    //    switch (spellBase.MetaSpellType)
    //    {
    //        case SpellType.Enchantment:
    //        case SpellType.FellowEnchantment:
    //            writer.Write((double)spellBase.Duration);
    //            writer.Write((float)spellBase.DegradeModifier);
    //            writer.Write((float)spellBase.DegradeLimit);
    //            break;
    //        case SpellType.PortalSummon:
    //            writer.Write((double)spellBase.PortalLifetime);
    //            break;
    //    }

    //    // TODO: Fix components
    //    for (int j = 0; j < 8; j++)
    //    {
    //        //if (j < spellBase.Formula.Count)
    //        //    writer.Write(spellBase.Formula[j]);
    //        //else
    //        writer.Write(0u);
    //    }

    //    writer.Write(spellBase.CasterEffect);

    //    writer.Write((float)spellBase.CasterEffect);
    //    writer.Write((float)spellBase.TargetEffect);
    //    writer.Write((float)spellBase.FizzleEffect);
    //    writer.Write((double)spellBase.RecoveryInterval);
    //    writer.Write((float)spellBase.RecoveryAmount);
    //    writer.Write(spellBase.DisplayOrder);
    //    writer.Write(spellBase.NonComponentTargetType);
    //    writer.Write(spellBase.ManaMod);

    //    ms.Position = 0;

    //    clone.Unpack(reader);

    //    return clone;
    //}

    ///// <summary>
    ///// Writes a string the way SpellBase reads it
    ///// </summary>
    //private static void WriteObfuscatedString(this BinaryWriter writer, string spellBase)
    //{
    //    writer.Write((uint)spellBase.Length);
    //    //System.Text.Encoding.GetEncoding(1252).GetString(spellBase.Name);
    //    for (var i = 0; i < spellBase.Length; i++)
    //    {
    //        // flip the bytes in the string to redo the obfuscation: i.e. 0xAB => 0xBA
    //        byte obfsByte = (byte)(spellBase[i] >> 4 | spellBase[i] << 4);
    //        writer.Write(obfsByte);
    //    }

    //    // Aligns to the next DWORD boundary.
    //    writer.Align();
    //    //long alignDelta = writer.BaseStream.Position % 4;

    //    //for (var i = 0; i < writer.BaseStream.Position % 4; i++)
    //    //    writer.Write((byte)0);
    //}

    ///// <summary>
    ///// Perform a deep Copy of the object, using Json as a serialization method. NOTE: Private members are not cloned using this method.
    ///// </summary>
    ///// <typeparam name="T">The type of object being copied.</typeparam>
    ///// <param name="source">The object instance to copy.</param>
    ///// <returns>The copied object.</returns>
    ////public static T CloneJson<T>(this T source)
    ////{
    ////    // Don't serialize a null object, simply return the default for that object
    ////    if (ReferenceEquals(source, null)) return default;

    ////    // initialize inner objects individually
    ////    // for example in default constructor some list property initialized with some values,
    ////    // but in 'source' these items are cleaned -
    ////    // without ObjectCreationHandling.Replace default constructor values will be added to result
    ////    var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

    ////    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
    ////}

    public static T Clone<T>(this T source) //where T : ISerializable
    {
        var serialized = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialized);
    }
    #endregion
}
