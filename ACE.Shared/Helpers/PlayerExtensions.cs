using ACE.DatLoader.FileTypes;
using ACE.Entity;
using ACE.Entity.Enum.Properties;
using ACE.Server.Command.Handlers;
using ACE.Server.Network.GameMessages.Messages;

namespace ACE.Shared.Helpers;

public static class PlayerExtensions
{
    private static PositionType[] wipedPositions =
{
        PositionType.Undef,
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
    public static void QuarantinePlayer(this Player player)
    {
        //Wipe positions
        foreach (var position in wipedPositions)
            player.SetPosition(position, null);

        var loc = "0x00070270 120 -80 18 -0.70710700750351 0 0 -0.70710700750351".Split(' ');
        AdminCommands.HandleTeleportLOC(player.Session, loc);
        //player.Teleport(new ACE.Entity.Position(0x00070270, 120, -80, 18, -0.70710700750351 0 0 - 0.70710700750351));
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
}
