using ACE.DatLoader.FileTypes;

namespace ACE.Shared.Helpers;

public static class PlayerExtensions
{
    /// <summary>
    /// Try to trains or specializes a skill depending on its status
    /// </summary>
    public static bool TryAdvanceSkill(this Player player, Skill skill)
    {
        if (player.GetCreatureSkill(skill) is not CreatureSkill s)
            return false;

        bool success = s.AdvancementClass == SkillAdvancementClass.Trained ?
            player.SpecializeSkill(skill) : player.TrainSkill(skill);

        return success;
    }

    private static PositionType[] wipedPositions =
    {
        PositionType.Home,
        PositionType.Instantiation,
        PositionType.LastOutsideDeath,
        PositionType.LastPortal,
        PositionType.LinkedLifestone,
        PositionType.LinkedPortalOne,
        PositionType.LinkedPortalTwo,
        PositionType.PortalSummonLoc,
        PositionType.Sanctuary,
    };
    public static void QuarantinePlayer(this Player player, string coords = "0x010D0100 -1.705717 2.126705 0.005000 0.577563 0.000000 0.000000 -0.816346")
    {
        //if (!CommandParameterHelpers.TryParsePosition(coords.Split(' '), out var error, out var newPos))
#if REALM
        if (!coords.TryParsePosition(out var newPos, player.Location.Instance))
#else
        if (!coords.TryParsePosition(out var newPos))
#endif
        {
            player.SendMessage($"Bad coordinates to quarantine to: {coords}");
            return;
        }

        //Wipe positions?
        foreach (var pos in wipedPositions) 
            player.SetPosition(pos, null);

        //Set home?
#if REALM
        player.LinkedLifestone = newPos.AsLocalPosition();
        player.Sanctuary = newPos.AsLocalPosition();
        player.Home = newPos; //.AsInstancedPosition(player, Entity.Enum.RealmProperties.PlayerInstanceSelectMode.HomeRealm);
#else
        player.SetPosition(PositionType.LinkedLifestone, newPos);
        player.SetPosition(PositionType.Sanctuary, newPos);
        player.SetPosition(PositionType.Home, newPos);
#endif

        player.RecallsDisabled = true;

        //Set flag used to prevent tele/other stuff?
        player.SetProperty(FakeBool.Quarantined, true);

        //Todo: determine how to see if player is 
        //player.TeleportThreadSafe(newPos.AsInstancedPosition(player, Entity.Enum.RealmProperties.PlayerInstanceSelectMode.HomeRealm));
        player.TeleportThreadSafe(newPos);
    }

    /// <summary>
    /// Log off and permanently delete the player
    /// </summary>
    /// <param name="player"></param>
    public static void PermaDeath(this Player player)
    {
        //Taken from /deletecharacter
        player.Character.DeleteTime = (ulong)Time.GetUnixTime();
        player.Character.IsDeleted = true;
        player.CharacterChangesDetected = true;
        player.Session.LogOffPlayer(true);
        PlayerManager.HandlePlayerDelete(player.Character.Id);

        var success = PlayerManager.ProcessDeletedPlayer(player.Character.Id);
        if (success)
            ModManager.Log($"Successfully deleted character {player.Name} (0x{player.Guid}).");
        else
            ModManager.Log($"Unable to delete character {player.Name} (0x{player.Guid}) due to PlayerManager failure.");
    }

    public static void CloneAppearance(this Player source, Creature target)
    {
        if (source is null || target is null)
            return;

        var cg = DatManager.PortalDat.CharGen;

        //Parse missing heritages?
        if (!source.Heritage.HasValue)
        {
            if (!string.IsNullOrEmpty(source.HeritageGroupName) && Enum.TryParse(source.HeritageGroupName.Replace("'", ""), true, out HeritageGroup heritage))
                source.Heritage = (int)heritage;
        }
        if (!target.Heritage.HasValue)
        {
            if (!string.IsNullOrEmpty(target.HeritageGroupName) && Enum.TryParse(target.HeritageGroupName.Replace("'", ""), true, out HeritageGroup heritage2))
                target.Heritage = (int)heritage2;
        }

        //Parse missing genders?
        if (!source.Gender.HasValue)
        {
            if (!string.IsNullOrEmpty(source.Sex) && Enum.TryParse(source.Sex, true, out Gender gender))
                source.Gender = (int)gender;
        }
        if (!target.Gender.HasValue)
        {
            if (!string.IsNullOrEmpty(target.Sex) && Enum.TryParse(target.Sex, true, out Gender gender2))
                target.Gender = (int)gender2;
        }

        if (!source.Heritage.HasValue || !source.Gender.HasValue || !target.Heritage.HasValue || !target.Gender.HasValue)
            return;

        //Copy gender/heritage
        target.Heritage = source.Heritage;
        target.Gender = source.Gender;          //Need HG to know if they have a gender?

        //Get heritage groups
        if (!cg.HeritageGroups.TryGetValue((uint)source.Heritage.Value, out var hg))
            return;
        if (!cg.HeritageGroups.TryGetValue((uint)target.Heritage.Value, out var hg2))
            return;

        //Copy
        target.HeritageGroup = (HeritageGroup)source.Heritage;
        target.HeritageGroupName = hg.Name;

        //Get sex
        if (!hg.Genders.TryGetValue((int)source.Gender, out var sex))
            return;

        //source.PaletteBaseId = sex.BasePalette;

        //var appearance = new Appearance
        //{
        //    HairStyle = 1,
        //    HairColor = 1,
        //    HairHue = 1,
        //    EyeColor = 1,
        //    Eyes = 1,
        //    Mouth = 1,
        //    Nose = 1,
        //    SkinHue = 1
        //};

        //// Get the hair first, because we need to know if you're bald, and that's the name of that tune!
        //if (sex.HairStyleList.Count > 1)
        //{
        //    if (PropertyManager.GetBool("npc_hairstyle_fullrange").Item)
        //        appearance.HairStyle = (uint)ThreadSafeRandom.Next(0, sex.HairStyleList.Count - 1);
        //    else
        //        appearance.HairStyle = (uint)ThreadSafeRandom.Next(0, Math.Min(sex.HairStyleList.Count - 1, 8)); // retail range data compiled by OptimShi
        //}
        //else
        //    appearance.HairStyle = 0;

        //if (sex.HairStyleList.Count < appearance.HairStyle)
        //    return;

        //var hairstyle = sex.HairStyleList[Convert.ToInt32(appearance.HairStyle)];

        //appearance.HairColor = (uint)ThreadSafeRandom.Next(0, sex.HairColorList.Count - 1);
        //appearance.HairHue = ThreadSafeRandom.Next(0.0f, 1.0f);
        //appearance.EyeColor = (uint)ThreadSafeRandom.Next(0, sex.EyeColorList.Count - 1);
        //appearance.Eyes = (uint)ThreadSafeRandom.Next(0, sex.EyeStripList.Count - 1);
        //appearance.Mouth = (uint)ThreadSafeRandom.Next(0, sex.MouthStripList.Count - 1);
        //appearance.Nose = (uint)ThreadSafeRandom.Next(0, sex.NoseStripList.Count - 1);
        //appearance.SkinHue = ThreadSafeRandom.Next(0.0f, 1.0f);

        //// Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
        ////if (hairstyle.AlternateSetup > 0)
        ////    character.SetupTableId = hairstyle.AlternateSetup;

        //source.EyesTextureDID = sex.GetEyeTexture(appearance.Eyes, hairstyle.Bald);
        //source.DefaultEyesTextureDID = sex.GetDefaultEyeTexture(appearance.Eyes, hairstyle.Bald);
        //source.NoseTextureDID = sex.GetNoseTexture(appearance.Nose);
        //source.DefaultNoseTextureDID = sex.GetDefaultNoseTexture(appearance.Nose);
        //source.MouthTextureDID = sex.GetMouthTexture(appearance.Mouth);
        //source.DefaultMouthTextureDID = sex.GetDefaultMouthTexture(appearance.Mouth);
        //source.HeadObjectDID = sex.GetHeadObject(appearance.HairStyle);

        //// Skin is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
        //var skinPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.SkinPalSet);
        //source.SkinPaletteDID = skinPalSet.GetPaletteID(appearance.SkinHue);

        //// Hair is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
        //var hairPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.HairColorList[Convert.ToInt32(appearance.HairColor)]);
        //source.HairPaletteDID = hairPalSet.GetPaletteID(appearance.HairHue);

        //// Eye Color
        //source.EyesPaletteDID = sex.EyeColorList[Convert.ToInt32(appearance.EyeColor)];

        //// pull character data from the dat file
        //source.SetProperty(PropertyDataId.MotionTable, sex.MotionTable);
        //source.SetProperty(PropertyDataId.SoundTable, sex.SoundTable);
        //source.SetProperty(PropertyDataId.PhysicsEffectTable, sex.PhysicsTable);
        //source.SetProperty(PropertyDataId.Setup, sex.SetupID);
        //source.SetProperty(PropertyDataId.PaletteBase, sex.BasePalette);
        //source.SetProperty(PropertyDataId.CombatTable, sex.CombatTable);

        //// Check the character scale
        //if (sex.Scale != 100)
        //    source.SetProperty(PropertyFloat.DefaultScale, sex.Scale / 100.0f); // Scale is stored as a percentage

        //// Olthoi and Gear Knights have a "Body Style" instead of a hair style. These styles have multiple model/texture changes, instead of a single head/hairstyle.
        //// Storing this value allows us to send the proper appearance ObjDesc
        //if (hairstyle.ObjDesc.AnimPartChanges.Count > 1)
        //    source.SetProperty(PropertyInt.Hairstyle, (int)appearance.HairStyle);

        //// Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
        //if (hairstyle.AlternateSetup > 0)
        //    source.SetProperty(PropertyDataId.Setup, hairstyle.AlternateSetup);

        //// HeadObject can be null if we're dealing with GearKnight or Olthoi
        //var headObject = sex.GetHeadObject(appearance.HairStyle);
        //if (headObject != null)
        //    source.SetProperty(PropertyDataId.HeadObject, (uint)headObject);
    }

    /// <summary>
    /// Pulled from PlayerFactor.Create and GenerateNewFace
    /// </summary>
    /// <param name="player"></param>
    public static void RandomizeAppearance(this Player player, bool randomizeHeritage = true, bool randomizeGender = true)
    {
        if (player is null)
            return;

        var cg = DatManager.PortalDat.CharGen;

        //Parse missing heritage
        if (!player.Heritage.HasValue)
        {
            if (!string.IsNullOrEmpty(player.HeritageGroupName) && Enum.TryParse(player.HeritageGroupName.Replace("'", ""), true, out HeritageGroup heritage))
                player.Heritage = (int)heritage;
        }

        //Parse missing gender?
        if (!player.Gender.HasValue)
        {
            if (!string.IsNullOrEmpty(player.Sex) && Enum.TryParse(player.Sex, true, out Gender gender))
                player.Gender = (int)gender;
        }

        if (!player.Heritage.HasValue || !player.Gender.HasValue)
            return;

        //Randomize gender/heritage
        if (randomizeHeritage)
            player.Heritage = ThreadSafeRandom.Next(1, 11);

        //Get heritage group
        if (!cg.HeritageGroups.TryGetValue((uint)player.Heritage.Value, out var hg))
            return;

        player.HeritageGroup = (HeritageGroup)player.Heritage;
        player.HeritageGroupName = hg.Name;

        //Need HG to know if they have a gender?
        //Randomize gender
        if (randomizeGender)
            player.Gender = randomizeGender ? hg.Genders.Keys.ToArray().Random() : 0;

        //Get sex
        if (!hg.Genders.TryGetValue((int)player.Gender, out var sex))
            return;

        player.PaletteBaseId = sex.BasePalette;

        var appearance = new Appearance
        {
            HairStyle = 1,
            HairColor = 1,
            HairHue = 1,
            EyeColor = 1,
            Eyes = 1,
            Mouth = 1,
            Nose = 1,
            SkinHue = 1
        };

        // Get the hair first, because we need to know if you're bald, and that's the name of that tune!
        if (sex.HairStyleList.Count > 1)
        {
            if (PropertyManager.GetBool("npc_hairstyle_fullrange").Item)
                appearance.HairStyle = (uint)ThreadSafeRandom.Next(0, sex.HairStyleList.Count - 1);
            else
                appearance.HairStyle = (uint)ThreadSafeRandom.Next(0, Math.Min(sex.HairStyleList.Count - 1, 8)); // retail range data compiled by OptimShi
        }
        else
            appearance.HairStyle = 0;

        if (sex.HairStyleList.Count < appearance.HairStyle)
            return;

        var hairstyle = sex.HairStyleList[Convert.ToInt32(appearance.HairStyle)];

        appearance.HairColor = (uint)ThreadSafeRandom.Next(0, sex.HairColorList.Count - 1);
        appearance.HairHue = ThreadSafeRandom.Next(0.0f, 1.0f);
        appearance.EyeColor = (uint)ThreadSafeRandom.Next(0, sex.EyeColorList.Count - 1);
        appearance.Eyes = (uint)ThreadSafeRandom.Next(0, sex.EyeStripList.Count - 1);
        appearance.Mouth = (uint)ThreadSafeRandom.Next(0, sex.MouthStripList.Count - 1);
        appearance.Nose = (uint)ThreadSafeRandom.Next(0, sex.NoseStripList.Count - 1);
        appearance.SkinHue = ThreadSafeRandom.Next(0.0f, 1.0f);

        // Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
        //if (hairstyle.AlternateSetup > 0)
        //    character.SetupTableId = hairstyle.AlternateSetup;

        player.EyesTextureDID = sex.GetEyeTexture(appearance.Eyes, hairstyle.Bald);
        player.DefaultEyesTextureDID = sex.GetDefaultEyeTexture(appearance.Eyes, hairstyle.Bald);
        player.NoseTextureDID = sex.GetNoseTexture(appearance.Nose);
        player.DefaultNoseTextureDID = sex.GetDefaultNoseTexture(appearance.Nose);
        player.MouthTextureDID = sex.GetMouthTexture(appearance.Mouth);
        player.DefaultMouthTextureDID = sex.GetDefaultMouthTexture(appearance.Mouth);
        player.HeadObjectDID = sex.GetHeadObject(appearance.HairStyle);

        // Skin is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
        var skinPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.SkinPalSet);
        player.SkinPaletteDID = skinPalSet.GetPaletteID(appearance.SkinHue);

        // Hair is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
        var hairPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.HairColorList[Convert.ToInt32(appearance.HairColor)]);
        player.HairPaletteDID = hairPalSet.GetPaletteID(appearance.HairHue);

        // Eye Color
        player.EyesPaletteDID = sex.EyeColorList[Convert.ToInt32(appearance.EyeColor)];

        // pull character data from the dat file
        player.SetProperty(PropertyDataId.MotionTable, sex.MotionTable);
        player.SetProperty(PropertyDataId.SoundTable, sex.SoundTable);
        player.SetProperty(PropertyDataId.PhysicsEffectTable, sex.PhysicsTable);
        player.SetProperty(PropertyDataId.Setup, sex.SetupID);
        player.SetProperty(PropertyDataId.PaletteBase, sex.BasePalette);
        player.SetProperty(PropertyDataId.CombatTable, sex.CombatTable);

        // Check the character scale
        if (sex.Scale != 100)
            player.SetProperty(PropertyFloat.DefaultScale, sex.Scale / 100.0f); // Scale is stored as a percentage

        // Olthoi and Gear Knights have a "Body Style" instead of a hair style. These styles have multiple model/texture changes, instead of a single head/hairstyle.
        // Storing this value allows us to send the proper appearance ObjDesc
        if (hairstyle.ObjDesc.AnimPartChanges.Count > 1)
            player.SetProperty(PropertyInt.Hairstyle, (int)appearance.HairStyle);

        // Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
        if (hairstyle.AlternateSetup > 0)
            player.SetProperty(PropertyDataId.Setup, hairstyle.AlternateSetup);

        // HeadObject can be null if we're dealing with GearKnight or Olthoi
        var headObject = sex.GetHeadObject(appearance.HairStyle);
        if (headObject != null)
            player.SetProperty(PropertyDataId.HeadObject, (uint)headObject);
    }

    public static void PlaySound(this Player player, Sound sound, float volume = 1f) =>
        player.EnqueueBroadcast(new GameMessageSound(player.Guid, sound, volume));

#if REALM
    public static void TeleportThreadSafe(this Player player, InstancedPosition position, bool fromInstance = true) => WorldManager.ThreadSafeTeleport(player, position, fromInstance);
#else
    public static void TeleportThreadSafe(this Player player, Position position) => WorldManager.ThreadSafeTeleport(player, position);
#endif
}
