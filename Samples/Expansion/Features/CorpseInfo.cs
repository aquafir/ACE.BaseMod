namespace Expansion.Features;

#if REALM

#else
//Corpse generation sucks.  Long method and it doesn't track the type of corpse.
[CommandCategory(nameof(Feature.CorpseInfo))]
[HarmonyPatchCategory(nameof(Feature.CorpseInfo))]
public class CorpseInfo
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Creature), nameof(Creature.CreateCorpse), new Type[] { typeof(DamageHistoryInfo), typeof(bool) })]
    public static bool PreCreateCorpse(DamageHistoryInfo killer, bool hadVitae, ref Creature __instance)
    {
        //        Debugger.Break();
        if (__instance.NoCorpse)
        {
            if (killer != null && killer.IsOlthoiPlayer) return false; ;

            var loot = __instance.GenerateTreasure(killer, null);

            foreach (var item in loot)
            {
                if (!string.IsNullOrEmpty(item.Quest)) // if the item has a Quest string, make the creature a "generator" of the item so that the pickup action applies the quest. 
                    item.GeneratorId = __instance.Guid.Full;
                item.Location = new Position(__instance.Location);
                LandblockManager.AddObject(item);
            }
            return false;
        }

        var cachedWeenie = DatabaseManager.World.GetCachedWeenie("corpse");

        var corpse = WorldObjectFactory.CreateNewWorldObject(cachedWeenie) as Corpse;








        //!!Add the functionality needed!!  Todo: clean alllll this up
        //        corpse.SetLivingWeenieType(__instance);
        corpse.SetProperty(FakeInt.CorpseLivingWCID, (int)__instance.WeenieClassId);
        corpse.SetProperty(FakeDID.CorpseLandblockId, __instance.CurrentLandblock.Id.Raw);
        corpse.SetProperty(FakeBool.CorpseSpawnedDungeon, __instance.CurrentLandblock.IsDungeon);




        var prefix = "Corpse";

        if (__instance.TreasureCorpse)
        {
            // Hardcoded values from PCAPs of Treasure Pile Corpses, everything else lines up exactly with existing corpse weenie
            corpse.SetupTableId = 0x02000EC4;
            corpse.MotionTableId = 0x0900019B;
            corpse.SoundTableId = 0x200000C2;
            corpse.ObjScale = 0.4f;

            prefix = "Treasure";
        }
        else
        {
            corpse.SetupTableId = __instance.SetupTableId;
            corpse.MotionTableId = __instance.MotionTableId;
            //corpse.SoundTableId = SoundTableId; // Do not change sound table for corpses
            corpse.PaletteBaseDID = __instance.PaletteBaseDID;
            corpse.ClothingBase = __instance.ClothingBase;
            corpse.PhysicsTableId = __instance.PhysicsTableId;

            if (__instance.ObjScale.HasValue)
                corpse.ObjScale = __instance.ObjScale;
            if (__instance.PaletteTemplate.HasValue)
                corpse.PaletteTemplate = __instance.PaletteTemplate;
            if (__instance.Shade.HasValue)
                corpse.Shade = __instance.Shade;
            //if (Translucency.HasValue) // Shadows have Translucency but their corpses do not, videographic evidence can be found on YouTube.
            //corpse.Translucency = Translucency;


            // Pull and save objdesc for correct corpse apperance at time of death
            var objDesc = __instance.CalculateObjDesc();

            corpse.Biota.PropertiesAnimPart = objDesc.AnimPartChanges.Clone(corpse.BiotaDatabaseLock);
            corpse.Biota.PropertiesPalette = objDesc.SubPalettes.Clone(corpse.BiotaDatabaseLock);
            corpse.Biota.PropertiesTextureMap = objDesc.TextureChanges.Clone(corpse.BiotaDatabaseLock);
        }

        // use the physics location for accuracy,
        // especially while jumping
        corpse.Location = __instance.PhysicsObj.Position.ACEPosition();

        corpse.VictimId = __instance.Guid.Full;
        corpse.Name = $"{prefix} of {__instance.Name}";

        // set 'killed by' for looting rights
        var killerName = "misadventure";
        if (killer != null)
        {
            if (!(__instance.Generator != null && __instance.Generator.Guid == killer.Guid) && __instance.Guid != killer.Guid)
            {
                if (!string.IsNullOrWhiteSpace(killer.Name))
                    killerName = killer.Name.TrimStart('+');  // vtank requires + to be stripped for regex matching.

                corpse.KillerId = killer.Guid.Full;

                if (killer.PetOwner != null)
                {
                    var petOwner = killer.TryGetPetOwner();
                    if (petOwner != null)
                        corpse.KillerId = petOwner.Guid.Full;
                }
            }
        }

        corpse.LongDesc = $"Killed by {killerName}.";

        bool saveCorpse = false;

        var player = __instance as Player;

        if (player != null)
        {
            corpse.SetPosition(PositionType.Location, corpse.Location);

            var killerIsOlthoiPlayer = killer != null && killer.IsOlthoiPlayer;
            var killerIsPkPlayer = killer != null && killer.IsPlayer && killer.Guid != __instance.Guid;

            //var dropped = killer != null && killer.IsOlthoiPlayer ? player.CalculateDeathItems_Olthoi(corpse, hadVitae) : player.CalculateDeathItems(corpse);

            if (killerIsOlthoiPlayer || player.IsOlthoiPlayer)
            {
                var dropped = player.CalculateDeathItems_Olthoi(corpse, hadVitae, killerIsOlthoiPlayer, killerIsPkPlayer);

                foreach (var wo in dropped)
                    __instance.DoCantripLogging(killer, wo);

                corpse.RecalculateDecayTime(player);

                if (dropped.Count > 0)
                    saveCorpse = true;

                corpse.PkLevel = PKLevel.PK;
            }
            else
            {
                var dropped = player.CalculateDeathItems(corpse);

                corpse.RecalculateDecayTime(player);

                if (dropped.Count > 0)
                    saveCorpse = true;

                if ((player.Location.Cell & 0xFFFF) < 0x100)
                {
                    player.SetPosition(PositionType.LastOutsideDeath, new Position(corpse.Location));
                    player.Session.Network.EnqueueSend(new GameMessagePrivateUpdatePosition(player, PositionType.LastOutsideDeath, corpse.Location));

                    if (dropped.Count > 0)
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat($"Your corpse is located at ({corpse.Location.GetMapCoordStr()}).", ChatMessageType.Broadcast));
                }

                var isPKdeath = player.IsPKDeath(killer);
                var isPKLdeath = player.IsPKLiteDeath(killer);

                if (isPKdeath)
                    corpse.PkLevel = PKLevel.PK;

                if (!isPKdeath && !isPKLdeath)
                {
                    var miserAug = player.AugmentationLessDeathItemLoss * 5;
                    if (miserAug > 0)
                        player.Session.Network.EnqueueSend(new GameMessageSystemChat($"Your augmentation has reduced the number of items you can lose by {miserAug}!", ChatMessageType.Broadcast));
                }

                if (dropped.Count == 0 && !isPKLdeath)
                    player.Session.Network.EnqueueSend(new GameMessageSystemChat($"You have retained all your items. You do not need to recover your corpse!", ChatMessageType.Broadcast));
            }
        }
        else
        {
            corpse.IsMonster = true;

            if (killer == null || !killer.IsOlthoiPlayer)
                __instance.GenerateTreasure(killer, corpse);
            else
                __instance.GenerateTreasure_Olthoi(killer, corpse);

            if (killer != null && killer.IsPlayer && !killer.IsOlthoiPlayer)
            {
                if (__instance.Level >= 100)
                {
                    __instance.CanGenerateRare = true;
                }
                else
                {
                    var killerPlayer = killer.TryGetAttacker();
                    if (killerPlayer != null && __instance.Level > killerPlayer.Level)
                        __instance.CanGenerateRare = true;
                }
            }
            else
                __instance.CanGenerateRare = false;
        }

        corpse.RemoveProperty(PropertyInt.Value);

        if (__instance.CanGenerateRare && killer != null)
            corpse.TryGenerateRare(killer);

        corpse.InitPhysicsObj();

        // persist the original creature velocity (only used for falling) to corpse
        corpse.PhysicsObj.Velocity = __instance.PhysicsObj.Velocity;


        corpse.EnterWorld();

        if (player != null)
        {
            //Todo: figure out issue with this!
            //if (corpse.PhysicsObj == null || corpse.PhysicsObj.Position == null)
            //    log.Debug($"[CORPSE] {Name}'s corpse (0x{corpse.Guid}) failed to spawn! Tried at {player.Location.ToLOCString()}");
            //else
            //    log.Debug($"[CORPSE] {Name}'s corpse (0x{corpse.Guid}) is located at {corpse.PhysicsObj.Position}");
        }

        if (saveCorpse)
        {
            corpse.SaveBiotaToDatabase();

            foreach (var item in corpse.Inventory.Values)
                item.SaveBiotaToDatabase();
        }

        //Skip original
        return false;
    }
}
#endif
