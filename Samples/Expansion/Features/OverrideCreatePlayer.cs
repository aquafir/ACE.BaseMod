//using ACE.Database;
//using ACE.DatLoader.FileTypes;
//using ACE.DatLoader;
//using static ACE.Server.Factories.PlayerFactory;

//namespace Expansion.Features;

//[CommandCategory(nameof(Feature.OverrideCreatePlayer))]
//[HarmonyPatchCategory(nameof(Feature.OverrideCreatePlayer))]
//internal class OverrideCreatePlayer
//{
//    [HarmonyPrefix]
//    [HarmonyPatch(typeof(PlayerFactory), nameof(PlayerFactory.Create), new Type[] { typeof(CharacterCreateInfo), typeof(Weenie), typeof(ObjectGuid), typeof(uint), typeof(WeenieType), typeof(Player) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
//    public static bool PreCreate(CharacterCreateInfo characterCreateInfo, Weenie weenie, ObjectGuid guid, uint accountId, WeenieType weenieType, Player player, ref CreateResult __result)
//    {
//        var heritageGroup = DatManager.PortalDat.CharGen.HeritageGroups[(uint)characterCreateInfo.Heritage];

//        if (weenieType == WeenieType.Admin)
//            player = new Admin(weenie, guid, accountId);
//        else if (weenieType == WeenieType.Sentinel)
//            player = new Sentinel(weenie, guid, accountId);
//        else
//            player = new Player(weenie, guid, accountId, RealmManager.DefaultRealmConfigured.StandardRules);

//        player.SetProperty(PropertyInt.HeritageGroup, (int)characterCreateInfo.Heritage);
//        player.SetProperty(PropertyString.HeritageGroup, heritageGroup.Name);
//        player.SetProperty(PropertyInt.Gender, (int)characterCreateInfo.Gender);
//        player.SetProperty(PropertyString.Sex, characterCreateInfo.Gender == 1 ? "Male" : "Female");

//        //player.SetProperty(PropertyDataId.Icon, cgh.IconImage); // I don't believe this is used anywhere in the client, but it might be used by a future custom launcher

//        // pull character data from the dat file
//        var sex = heritageGroup.Genders[(int)characterCreateInfo.Gender];

//        player.SetProperty(PropertyDataId.MotionTable, sex.MotionTable);
//        player.SetProperty(PropertyDataId.SoundTable, sex.SoundTable);
//        player.SetProperty(PropertyDataId.PhysicsEffectTable, sex.PhysicsTable);
//        player.SetProperty(PropertyDataId.Setup, sex.SetupID);
//        player.SetProperty(PropertyDataId.PaletteBase, sex.BasePalette);
//        player.SetProperty(PropertyDataId.CombatTable, sex.CombatTable);

//        // Check the character scale
//        if (sex.Scale != 100)
//            player.SetProperty(PropertyFloat.DefaultScale, sex.Scale / 100.0f); // Scale is stored as a percentage

//        // Get the hair first, because we need to know if you're bald, and that's the name of that tune!
//        var hairstyle = sex.HairStyleList[(int)characterCreateInfo.Appearance.HairStyle];

//        // Olthoi and Gear Knights have a "Body Style" instead of a hair style. These styles have multiple model/texture changes, instead of a single head/hairstyle.
//        // Storing this value allows us to send the proper appearance ObjDesc
//        if (hairstyle.ObjDesc.AnimPartChanges.Count > 1)
//            player.SetProperty(PropertyInt.Hairstyle, (int)characterCreateInfo.Appearance.HairStyle);

//        // Certain races (Undead, Tumeroks, Others?) have multiple body styles available. This is controlled via the "hair style".
//        if (hairstyle.AlternateSetup > 0)
//            player.SetProperty(PropertyDataId.Setup, hairstyle.AlternateSetup);

//        player.SetProperty(PropertyDataId.EyesTexture, sex.GetEyeTexture(characterCreateInfo.Appearance.Eyes, hairstyle.Bald));
//        player.SetProperty(PropertyDataId.DefaultEyesTexture, sex.GetDefaultEyeTexture(characterCreateInfo.Appearance.Eyes, hairstyle.Bald));
//        player.SetProperty(PropertyDataId.NoseTexture, sex.GetNoseTexture(characterCreateInfo.Appearance.Nose));
//        player.SetProperty(PropertyDataId.DefaultNoseTexture, sex.GetDefaultNoseTexture(characterCreateInfo.Appearance.Nose));
//        player.SetProperty(PropertyDataId.MouthTexture, sex.GetMouthTexture(characterCreateInfo.Appearance.Mouth));
//        player.SetProperty(PropertyDataId.DefaultMouthTexture, sex.GetDefaultMouthTexture(characterCreateInfo.Appearance.Mouth));
//        player.Character.HairTexture = sex.GetHairTexture(characterCreateInfo.Appearance.HairStyle) ?? 0;
//        player.Character.DefaultHairTexture = sex.GetDefaultHairTexture(characterCreateInfo.Appearance.HairStyle) ?? 0;
//        // HeadObject can be null if we're dealing with GearKnight or Olthoi
//        var headObject = sex.GetHeadObject(characterCreateInfo.Appearance.HairStyle);
//        if (headObject != null)
//            player.SetProperty(PropertyDataId.HeadObject, (uint)headObject);

//        // Skin is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
//        var skinPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.SkinPalSet);
//        player.SetProperty(PropertyDataId.SkinPalette, skinPalSet.GetPaletteID(characterCreateInfo.Appearance.SkinHue));
//        player.SetProperty(PropertyFloat.Shade, characterCreateInfo.Appearance.SkinHue);

//        // Hair is stored as PaletteSet (list of Palettes), so we need to read in the set to get the specific palette
//        var hairPalSet = DatManager.PortalDat.ReadFromDat<PaletteSet>(sex.HairColorList[(int)characterCreateInfo.Appearance.HairColor]);
//        player.SetProperty(PropertyDataId.HairPalette, hairPalSet.GetPaletteID(characterCreateInfo.Appearance.HairHue));

//        // Eye Color
//        player.SetProperty(PropertyDataId.EyesPalette, sex.EyeColorList[(int)characterCreateInfo.Appearance.EyeColor]);

//        // skip over this for olthoi, use the weenie defaults
//        if (!player.IsOlthoiPlayer)
//        {
//            if (player.Heritage != (int)HeritageGroup.Gearknight) // Gear Knights do not get clothing (pcap verified)
//            {
//                if (characterCreateInfo.Appearance.HeadgearStyle < uint.MaxValue) // No headgear is max UINT
//                {
//                    var hat = GetClothingObject(sex.GetHeadgearWeenie(characterCreateInfo.Appearance.HeadgearStyle), characterCreateInfo.Appearance.HeadgearColor, characterCreateInfo.Appearance.HeadgearHue);
//                    if (hat != null)
//                        player.TryEquipObject(hat, hat.ValidLocations ?? 0);
//                    else
//                        player.TryAddToInventory(CreateIOU(sex.GetHeadgearWeenie(characterCreateInfo.Appearance.HeadgearStyle)));
//                }

//                var shirt = GetClothingObject(sex.GetShirtWeenie(characterCreateInfo.Appearance.ShirtStyle), characterCreateInfo.Appearance.ShirtColor, characterCreateInfo.Appearance.ShirtHue);
//                if (shirt != null)
//                    player.TryEquipObject(shirt, shirt.ValidLocations ?? 0);
//                else
//                    player.TryAddToInventory(CreateIOU(sex.GetShirtWeenie(characterCreateInfo.Appearance.ShirtStyle)));

//                var pants = GetClothingObject(sex.GetPantsWeenie(characterCreateInfo.Appearance.PantsStyle), characterCreateInfo.Appearance.PantsColor, characterCreateInfo.Appearance.PantsHue);
//                if (pants != null)
//                    player.TryEquipObject(pants, pants.ValidLocations ?? 0);
//                else
//                    player.TryAddToInventory(CreateIOU(sex.GetPantsWeenie(characterCreateInfo.Appearance.PantsStyle)));

//                var shoes = GetClothingObject(sex.GetFootwearWeenie(characterCreateInfo.Appearance.FootwearStyle), characterCreateInfo.Appearance.FootwearColor, characterCreateInfo.Appearance.FootwearHue);
//                if (shoes != null)
//                    player.TryEquipObject(shoes, shoes.ValidLocations ?? 0);
//                else
//                    player.TryAddToInventory(CreateIOU(sex.GetFootwearWeenie(characterCreateInfo.Appearance.FootwearStyle)));
//            }

//            string templateName = heritageGroup.Templates[characterCreateInfo.TemplateOption].Name;
//            player.SetProperty(PropertyString.Template, templateName);

//            player.AddTitle(heritageGroup.Templates[characterCreateInfo.TemplateOption].Title, true);

//            // attributes
//            var result = ValidateAttributeCredits(characterCreateInfo, heritageGroup.AttributeCredits);

//            if (result != CreateResult.Success)
//            {
//                __result = result;
//                return false;
//            }

//            player.Strength.StartingValue = characterCreateInfo.StrengthAbility;
//            player.Endurance.StartingValue = characterCreateInfo.EnduranceAbility;
//            player.Coordination.StartingValue = characterCreateInfo.CoordinationAbility;
//            player.Quickness.StartingValue = characterCreateInfo.QuicknessAbility;
//            player.Focus.StartingValue = characterCreateInfo.FocusAbility;
//            player.Self.StartingValue = characterCreateInfo.SelfAbility;

//            // data we don't care about
//            //characterCreateInfo.CharacterSlot;
//            //characterCreateInfo.ClassId;

//            // characters start with max vitals
//            player.Health.Current = player.Health.Base;
//            player.Stamina.Current = player.Stamina.Base;
//            player.Mana.Current = player.Mana.Base;

//            // set initial skill credit amount. 52 for all but "Olthoi", which have 68
//            player.SetProperty(PropertyInt.AvailableSkillCredits, (int)heritageGroup.SkillCredits);
//            player.SetProperty(PropertyInt.TotalSkillCredits, (int)heritageGroup.SkillCredits);

//            if (characterCreateInfo.SkillAdvancementClasses.Count != 55)
//            {
//                __result = CreateResult.ClientServerSkillsMismatch;
//                return false;
//            }

//            for (int i = 0; i < characterCreateInfo.SkillAdvancementClasses.Count; i++)
//            {
//                var sac = characterCreateInfo.SkillAdvancementClasses[i];

//                if (sac == SkillAdvancementClass.Inactive)
//                    continue;

//                if (!DatManager.PortalDat.SkillTable.SkillBaseHash.ContainsKey((uint)i))
//                {
//                    __result = CreateResult.InvalidSkillRequested;
//                    return false;
//                }

//                var skill = DatManager.PortalDat.SkillTable.SkillBaseHash[(uint)i];

//                var trainedCost = skill.TrainedCost;
//                var specializedCost = skill.UpgradeCostFromTrainedToSpecialized;

//                foreach (var skillGroup in heritageGroup.Skills)
//                {
//                    if (skillGroup.SkillNum == i)
//                    {
//                        trainedCost = skillGroup.NormalCost;
//                        specializedCost = skillGroup.PrimaryCost;
//                        break;
//                    }
//                }

//                if (sac == SkillAdvancementClass.Specialized)
//                {
//                    if (!player.TrainSkill((Skill)i, trainedCost))
//                    {
//                        __result = CreateResult.FailedToTrainSkill;
//                        return false;
//                    }
//                    if (!player.SpecializeSkill((Skill)i, specializedCost))
//                    {
//                        __result = CreateResult.FailedToSpecializeSkill;
//                        return false;
//                    }
//                }
//                else if (sac == SkillAdvancementClass.Trained)
//                {
//                    if (!player.TrainSkill((Skill)i, trainedCost, true))
//                    {
//                        __result = CreateResult.FailedToTrainSkill;
//                        return false;
//                    }
//                }
//                else if (sac == SkillAdvancementClass.Untrained)
//                    player.UntrainSkill((Skill)i, 0);
//            }

//            // Set Heritage based Melee and Ranged Masteries
//            GetMasteries(player.HeritageGroup, out WeaponType meleeMastery, out WeaponType rangedMastery);

//            player.SetProperty(PropertyInt.MeleeMastery, (int)meleeMastery);
//            player.SetProperty(PropertyInt.RangedMastery, (int)rangedMastery);

//            // Set innate augs
//            SetInnateAugmentations(player);

//            HandleStarterGear(characterCreateInfo, player);
//        }
//        else
//        {
//            __result = CreateResult.InvalidSkillRequested;
//            return false;
//        }

//        player.Name = characterCreateInfo.Name;
//        player.Character.Name = characterCreateInfo.Name;


//        // Index used to determine the starting location
//        if (!ACRealmsConfigManager.Config.CharacterCreationOptions.UseRealmSelector)
//        {
//            WorldRealm defaultRealm;
//            if (ACRealmsConfigManager.Config.OptOutOfRealms)
//                defaultRealm = RealmManager.GetReservedRealm(ReservedRealm.@default);
//            else
//                defaultRealm = RealmManager.GetRealmByName(ACRealmsConfigManager.Config.DefaultRealm, includeRulesets: false);

//            var startArea = characterCreateInfo.StartArea;
//            var starterArea = DatManager.PortalDat.CharGen.StarterAreas[(int)startArea];
//            var startLoc = new LocalPosition(starterArea.Locations[0].ObjCellID,
//                starterArea.Locations[0].Frame.Origin.X, starterArea.Locations[0].Frame.Origin.Y, starterArea.Locations[0].Frame.Origin.Z,
//                starterArea.Locations[0].Frame.Orientation.X, starterArea.Locations[0].Frame.Orientation.Y, starterArea.Locations[0].Frame.Orientation.Z, starterArea.Locations[0].Frame.Orientation.W);
//            var iid = defaultRealm.StandardRules.GetDefaultInstanceID(player, startLoc);

//            RealmManager.SetHomeRealm(player, defaultRealm.Realm.Id, settingFromRealmSelector: false, saveImmediately: false); // Will crash if teleportToDefaultLoc is true
//            player.Location = new InstancedPosition(startLoc, iid);
//            if (!player.IsOlthoiPlayer)
//            {
//                player.Sanctuary = new InstancedPosition(player.Location).AsLocalPosition();
//                player.SetProperty(PropertyBool.RecallsDisabled, true);
//            }

//            var instantiation = new InstancedPosition(new LocalPosition(0xA9B40019, 84, 7.1f, 94, 0, 0, -0.0784591f, 0.996917f), iid); // ultimate fallback.

//            var spellFreeRide = new ACE.Database.Models.World.Spell();
//            switch (starterArea.Name)
//            {
//                case "OlthoiLair": //todo: check this when olthoi play is allowed in ace
//                    spellFreeRide = null; // no training area for olthoi, so they start and fall back to same place.
//                    instantiation = new InstancedPosition(player.Location);
//                    break;
//                case "Shoushi":
//                    spellFreeRide = DatabaseManager.World.GetCachedSpell(3813); // Free Ride to Shoushi
//                    break;
//                case "Yaraq":
//                    spellFreeRide = DatabaseManager.World.GetCachedSpell(3814); // Free Ride to Yaraq
//                    break;
//                case "Sanamar":
//                    spellFreeRide = DatabaseManager.World.GetCachedSpell(3535); // Free Ride to Sanamar
//                    break;
//                case "Holtburg":
//                default:
//                    spellFreeRide = DatabaseManager.World.GetCachedSpell(3815); // Free Ride to Holtburg
//                    break;
//            }
//            if (spellFreeRide != null && spellFreeRide.Name != "")
//                instantiation =
//                    new InstancedPosition(new LocalPosition(
//                        spellFreeRide.PositionObjCellId.Value, spellFreeRide.PositionOriginX.Value, spellFreeRide.PositionOriginY.Value, spellFreeRide.PositionOriginZ.Value,
//                        spellFreeRide.PositionAnglesX.Value, spellFreeRide.PositionAnglesY.Value, spellFreeRide.PositionAnglesZ.Value, spellFreeRide.PositionAnglesW.Value), iid);

//            player.Instantiation = new InstancedPosition(instantiation);
//        }
//        else
//        {
//            var realmSelector = RealmManager.GetReservedRealm(ReservedRealm.RealmSelector);
//            var blaineRoom = new LocalPosition(0x8903012E, 87.738312f, -47.704556f, .005f, 0f, 0f, -0.926821f, 0.375504f);
//            var iid = realmSelector.StandardRules.GetDefaultInstanceID(player, blaineRoom);
//            var startPos = blaineRoom.AsInstancedPosition(iid);
//            RealmManager.SetHomeRealm(player, realmSelector.Realm.Id, false, saveImmediately: false);
//            player.Location = startPos;
//            player.Instantiation = startPos;
//            player.Sanctuary = player.Location.AsLocalPosition();
//            player.SetProperty(PropertyBool.RecallsDisabled, true);

//            var token = WorldObjectFactory.CreateNewWorldObject((uint)ACE.Entity.Enum.WeenieClassName.W_TOKENTRAININGEXIT_CLASS, realmSelector.StandardRules);
//            if (token == null)
//                throw new InvalidOperationException("Academy Exit Token Weenie not found.");
//            player.TryAddToInventory(token);
//        }

//        if (player is Sentinel || player is Admin)
//        {
//            player.Character.IsPlussed = true;
//            player.CloakStatus = CloakStatus.Off;
//            player.ChannelsAllowed = player.ChannelsActive;
//        }

//        CharacterCreateSetDefaultCharacterOptions(player);

//        __result = CreateResult.Success;
//        return false;
//    }

//    private static void HandleStarterGear(CharacterCreateInfo characterCreateInfo, Player player)
//    {
//        var isDualWieldTrainedOrSpecialized = player.Skills.TryGetValue(Skill.DualWield, out var dualWield) && dualWield.AdvancementClass > SkillAdvancementClass.Untrained;

//        // grant starter items based on skills
//        var starterGearConfig = StarterGearFactory.GetStarterGearConfiguration();
//        var grantedWeenies = new List<uint>();

//        foreach (var skillGear in starterGearConfig.Skills)
//        {
//            //var charSkill = player.Skills[(Skill)skillGear.SkillId];
//            if (!player.Skills.TryGetValue((Skill)skillGear.SkillId, out var charSkill))
//                continue;

//            if (charSkill.AdvancementClass == SkillAdvancementClass.Trained || charSkill.AdvancementClass == SkillAdvancementClass.Specialized)
//            {
//                foreach (var item in skillGear.Gear)
//                {
//                    if (grantedWeenies.Contains(item.WeenieId))
//                    {
//                        var existingItem = player.Inventory.Values.FirstOrDefault(i => i.WeenieClassId == item.WeenieId);
//                        if (existingItem == null || (existingItem.MaxStackSize ?? 1) <= 1)
//                            continue;

//                        existingItem.SetStackSize(existingItem.StackSize + item.StackSize);
//                        continue;
//                    }

//                    var loot = WorldObjectFactory.CreateNewWorldObject(item.WeenieId);
//                    if (loot != null)
//                    {
//                        if (loot.StackSize.HasValue && loot.MaxStackSize.HasValue)
//                            loot.SetStackSize((item.StackSize <= loot.MaxStackSize) ? item.StackSize : loot.MaxStackSize);
//                    }
//                    else
//                    {
//                        player.TryAddToInventory(CreateIOU(item.WeenieId));
//                    }

//                    if (loot != null && player.TryAddToInventory(loot))
//                        grantedWeenies.Add(item.WeenieId);

//                    if (isDualWieldTrainedOrSpecialized && loot != null)
//                    {
//                        if (loot.WeenieType == WeenieType.MeleeWeapon)
//                        {
//                            var dualloot = WorldObjectFactory.CreateNewWorldObject(item.WeenieId);
//                            if (dualloot != null)
//                            {
//                                player.TryAddToInventory(dualloot);
//                            }
//                            else
//                            {
//                                player.TryAddToInventory(CreateIOU(item.WeenieId));
//                            }
//                        }
//                    }
//                }

//                var heritageLoot = skillGear.Heritage.FirstOrDefault(i => i.HeritageId == (ushort)characterCreateInfo.Heritage);
//                if (heritageLoot != null)
//                {
//                    foreach (var item in heritageLoot.Gear)
//                    {
//                        if (grantedWeenies.Contains(item.WeenieId))
//                        {
//                            var existingItem = player.Inventory.Values.FirstOrDefault(i => i.WeenieClassId == item.WeenieId);
//                            if (existingItem == null || (existingItem.MaxStackSize ?? 1) <= 1)
//                                continue;

//                            existingItem.SetStackSize(existingItem.StackSize + item.StackSize);
//                            continue;
//                        }

//                        var loot = WorldObjectFactory.CreateNewWorldObject(item.WeenieId);
//                        if (loot != null)
//                        {
//                            if (loot.StackSize.HasValue && loot.MaxStackSize.HasValue)
//                                loot.SetStackSize((item.StackSize <= loot.MaxStackSize) ? item.StackSize : loot.MaxStackSize);
//                        }
//                        else
//                        {
//                            player.TryAddToInventory(CreateIOU(item.WeenieId));
//                        }

//                        if (loot != null && player.TryAddToInventory(loot))
//                            grantedWeenies.Add(item.WeenieId);

//                        if (isDualWieldTrainedOrSpecialized && loot != null)
//                        {
//                            if (loot.WeenieType == WeenieType.MeleeWeapon)
//                            {
//                                var dualloot = WorldObjectFactory.CreateNewWorldObject(item.WeenieId);
//                                if (dualloot != null)
//                                {
//                                    player.TryAddToInventory(dualloot);
//                                }
//                                else
//                                {
//                                    player.TryAddToInventory(CreateIOU(item.WeenieId));
//                                }
//                            }
//                        }
//                    }
//                }

//                foreach (var spell in skillGear.Spells)
//                {
//                    if (charSkill.AdvancementClass == SkillAdvancementClass.Trained && spell.SpecializedOnly == false)
//                        player.AddKnownSpell(spell.SpellId);
//                    else if (charSkill.AdvancementClass == SkillAdvancementClass.Specialized)
//                        player.AddKnownSpell(spell.SpellId);
//                }
//            }
//        }
//    }
//}


