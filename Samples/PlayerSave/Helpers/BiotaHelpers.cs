using Biota = ACE.Database.Models.Shard.Biota;

namespace PlayerSave.Helpers;

public static class BiotaHelpers
{
    public static void Write(this List<Biota> biotas, BinaryWriter writer)
    {
        writer.Write(biotas != null);
        if (biotas is null)
            return;

        writer.Write(biotas.Count);

        foreach (var biota in biotas)
            biota.Write(writer);
    }
    public static void ReadBiotas(this List<Biota> biotas, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            Biota biota = new();
            biota.ReadBiota(reader);
            biotas.Add(biota);
        }
    }
    public static void Write(this Biota biota, BinaryWriter writer)
    {
        writer.Write(biota != null);
        if (biota is null)
            return;

        #region Shallow
        writer.Write(biota.Id);
        writer.Write(biota.WeenieClassId);
        writer.Write(biota.WeenieType);
        writer.Write(biota.PopulatedCollectionFlags);
        #endregion
        biota.BiotaPropertiesAnimPart.Write(writer);
        biota.BiotaPropertiesAttribute.Write(writer);
        biota.BiotaPropertiesAttribute2nd.Write(writer);
        biota.BiotaPropertiesBodyPart.Write(writer);
        biota.BiotaPropertiesBook.Write(writer);
        biota.BiotaPropertiesBookPageData.Write(writer);
        biota.BiotaPropertiesBool.Write(writer);
        biota.BiotaPropertiesCreateList.Write(writer);
        biota.BiotaPropertiesDID.Write(writer);
        biota.BiotaPropertiesEmote.Write(writer);
        biota.BiotaPropertiesEnchantmentRegistry.Write(writer);
        biota.BiotaPropertiesEventFilter.Write(writer);
        biota.BiotaPropertiesFloat.Write(writer);
        biota.BiotaPropertiesGenerator.Write(writer);
        biota.BiotaPropertiesIID.Write(writer);
        biota.BiotaPropertiesInt.Write(writer);
        biota.BiotaPropertiesInt64.Write(writer);
        biota.BiotaPropertiesPalette.Write(writer);
        biota.BiotaPropertiesPosition.Write(writer);
        biota.BiotaPropertiesSkill.Write(writer);
        biota.BiotaPropertiesSpellBook.Write(writer);
        biota.BiotaPropertiesString.Write(writer);
        biota.BiotaPropertiesTextureMap.Write(writer);

        //Allegiance and house would require extra handling
        biota.BiotaPropertiesAllegiance.Write(writer);
        biota.HousePermission.Write(writer);
    }
    public static void ReadBiota(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        //REQUIRES SAME ORDER AS WRITE
        #region Shallow
        biota.Id = reader.ReadUInt32();
        biota.WeenieClassId = reader.ReadUInt32();
        biota.WeenieType = reader.ReadInt32();
        biota.PopulatedCollectionFlags = reader.ReadUInt32();
        #endregion

        biota.ReadAnimParts(reader);
        biota.ReadAttributes(reader);
        biota.ReadAttribute2nds(reader);
        biota.ReadBodyParts(reader);
        biota.ReadBookProperties(reader);
        biota.ReadPages(reader);
        biota.ReadBools(reader);
        biota.ReadCreateList(reader);
        biota.ReadDIDs(reader);
        biota.ReadEmotes(reader);
        biota.ReadEnchantments(reader);
        biota.ReadEvents(reader);
        biota.ReadFloats(reader);
        biota.ReadGenerators(reader);
        biota.ReadIIDs(reader);
        biota.ReadInts(reader);
        biota.ReadInt64s(reader);
        biota.ReadPalettes(reader);
        biota.ReadPositions(reader);
        biota.ReadSkills(reader);
        biota.ReadSpellbook(reader);
        biota.ReadStrings(reader);
        biota.ReadTextureMaps(reader);

        //Allegiance and house require more work
        biota.ReadAllegianceData(reader);
        biota.ReadHousePermissions(reader);
    }

    //Todo: Decide what to do with Allegiance and Character reference
    public static void Write(this ICollection<BiotaPropertiesAllegiance> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            //writer.Write(prop.Allegiance); //Todo
            writer.Write(prop.AllegianceId);
            writer.Write(prop.ApprovedVassal);
            writer.Write(prop.Banned);
            //writer.Write(prop.Character);
            writer.Write(prop.CharacterId);
        }
    }
    public static void ReadAllegianceData(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesAllegiance is null)
            biota.BiotaPropertiesAllegiance = new List<BiotaPropertiesAllegiance>();

        var props = biota.BiotaPropertiesAllegiance;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesAllegiance
            {
                //Allegiance = reader.ReadUInt32(),
                AllegianceId = reader.ReadUInt32(),
                ApprovedVassal = reader.ReadBoolean(),
                Banned = reader.ReadBoolean(),
                //Character = reader.ReadUInt32(),
                CharacterId = reader.ReadUInt32(),
            };

            props.Add(value);
        }
    }
    //Todo: decide what to do with reference to House
    public static void Write(this ICollection<HousePermission> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            //writer.Write(prop.House);
            writer.Write(prop.HouseId);
            writer.Write(prop.PlayerGuid);
            writer.Write(prop.Storage);
        }
    }
    public static void ReadHousePermissions(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.HousePermission is null)
            biota.HousePermission = new List<HousePermission>();

        var props = biota.HousePermission;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new HousePermission
            {
                //House = //Todo
                HouseId = reader.ReadUInt32(),
                PlayerGuid = reader.ReadUInt32(),
                Storage = reader.ReadBoolean(),
            };

            props.Add(value);
        }
    }

    public static void Write(this ICollection<BiotaPropertiesAnimPart> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.AnimationId);
            writer.Write(prop.Id);
            writer.Write(prop.Index);
            writer.Write(prop.ObjectId);

            writer.Write(prop.Order != null);
            if (prop.Order != null)
                writer.Write(prop.Order.GetValueOrDefault());
        }
    }
    public static void ReadAnimParts(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;


        if (biota.BiotaPropertiesAnimPart is null)
            biota.BiotaPropertiesAnimPart = new List<BiotaPropertiesAnimPart>();

        var props = biota.BiotaPropertiesAnimPart;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesAnimPart
            {
                AnimationId = reader.ReadUInt32(),
                Id = reader.ReadUInt32(),
                Index = reader.ReadByte(),
                ObjectId = reader.ReadUInt32(),
                Object = biota,
            };

            if (reader.ReadBoolean())
                value.Order = reader.ReadByte();

            props.Add(value);
        }
    }
    public static void Write(this BiotaPropertiesBook props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.MaxNumCharsPerPage);
        writer.Write(props.MaxNumPages);
        writer.Write(props.ObjectId);
    }
    public static void ReadBookProperties(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesBook is null)
            biota.BiotaPropertiesBook = new();

        biota.BiotaPropertiesBook.MaxNumCharsPerPage = reader.ReadInt32();
        biota.BiotaPropertiesBook.MaxNumPages = reader.ReadInt32();
        biota.BiotaPropertiesBook.ObjectId = reader.ReadUInt32();
        biota.BiotaPropertiesBook.Object = biota;
    }
    public static void Write(this ICollection<BiotaPropertiesBookPageData> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.AuthorAccount);
            writer.Write(prop.AuthorId);
            writer.Write(prop.AuthorName);
            writer.Write(prop.Id);
            writer.Write(prop.IgnoreAuthor);
            writer.Write(prop.ObjectId);
            writer.Write(prop.PageId);
            writer.Write(prop.PageText);
        }
    }
    public static void ReadPages(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesBookPageData is null)
            biota.BiotaPropertiesBookPageData = new List<BiotaPropertiesBookPageData>();

        var props = biota.BiotaPropertiesBookPageData;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesBookPageData
            {
                AuthorAccount = reader.ReadString(),
                AuthorId = reader.ReadUInt32(),
                AuthorName = reader.ReadString(),
                Id = reader.ReadUInt32(),
                IgnoreAuthor = reader.ReadBoolean(),
                ObjectId = reader.ReadUInt32(),
                PageId = reader.ReadUInt32(),
                PageText = reader.ReadString(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesCreateList> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.DestinationType);
            writer.Write(prop.Id);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Palette);
            writer.Write(prop.Shade);
            writer.Write(prop.StackSize);
            writer.Write(prop.TryToBond);
            writer.Write(prop.WeenieClassId);
        }
    }
    public static void ReadCreateList(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesCreateList is null)
            biota.BiotaPropertiesCreateList = new List<BiotaPropertiesCreateList>();

        var props = biota.BiotaPropertiesCreateList;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesCreateList
            {
                DestinationType = reader.ReadSByte(),
                Id = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                Palette = reader.ReadSByte(),
                Shade = reader.ReadUInt32(),
                StackSize = reader.ReadInt32(),
                TryToBond = reader.ReadBoolean(),
                WeenieClassId = reader.ReadUInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesEnchantmentRegistry> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.CasterObjectId);
            writer.Write(prop.DegradeLimit);
            writer.Write(prop.DegradeModifier);
            writer.Write(prop.Duration);
            writer.Write(prop.EnchantmentCategory);
            writer.Write(prop.HasSpellSetId);
            writer.Write(prop.LastTimeDegraded);
            writer.Write(prop.LayerId);
            writer.Write(prop.ObjectId);
            writer.Write(prop.PowerLevel);
            writer.Write(prop.SpellCategory);
            writer.Write(prop.SpellId);
            writer.Write(prop.SpellSetId);
            writer.Write(prop.StartTime);
            writer.Write(prop.StatModKey);
            writer.Write(prop.StatModType);
            writer.Write(prop.StatModValue);
        }
    }
    public static void ReadEnchantments(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesEnchantmentRegistry is null)
            biota.BiotaPropertiesEnchantmentRegistry = new List<BiotaPropertiesEnchantmentRegistry>();

        var props = biota.BiotaPropertiesEnchantmentRegistry;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesEnchantmentRegistry
            {
                CasterObjectId = reader.ReadUInt32(),
                DegradeLimit = reader.ReadSingle(),
                DegradeModifier = reader.ReadSingle(),
                Duration = reader.ReadDouble(),
                EnchantmentCategory = reader.ReadUInt32(),
                HasSpellSetId = reader.ReadBoolean(),
                LastTimeDegraded = reader.ReadDouble(),
                LayerId = reader.ReadUInt16(),
                ObjectId = reader.ReadUInt32(),
                PowerLevel = reader.ReadUInt32(),
                SpellCategory = reader.ReadUInt16(),
                SpellId = reader.ReadInt32(),
                SpellSetId = reader.ReadUInt32(),
                StartTime = reader.ReadDouble(),
                StatModKey = reader.ReadUInt32(),
                StatModType = reader.ReadUInt32(),
                StatModValue = reader.ReadSingle(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesEventFilter> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Event);
            writer.Write(prop.ObjectId);
        }
    }
    public static void ReadEvents(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesEventFilter is null)
            biota.BiotaPropertiesEventFilter = new List<BiotaPropertiesEventFilter>();

        var props = biota.BiotaPropertiesEventFilter;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesEventFilter
            {
                Event = reader.ReadInt32(),
                ObjectId = reader.ReadUInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesGenerator> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Id);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Probability);
            writer.Write(prop.WeenieClassId);
            writer.Write(prop.InitCreate);
            writer.Write(prop.MaxCreate);
            writer.Write(prop.WhenCreate);
            writer.Write(prop.WhereCreate);

            writer.Write(prop.Delay != null);
            if (prop.Delay != null)
                writer.Write(prop.Delay.GetValueOrDefault());
            writer.Write(prop.StackSize != null);
            if (prop.StackSize != null)
                writer.Write(prop.StackSize.GetValueOrDefault());
            writer.Write(prop.PaletteId != null);
            if (prop.PaletteId != null)
                writer.Write(prop.PaletteId.GetValueOrDefault());
            writer.Write(prop.Shade != null);
            if (prop.Shade != null)
                writer.Write(prop.Shade.GetValueOrDefault());
            writer.Write(prop.ObjCellId != null);
            if (prop.ObjCellId != null)
                writer.Write(prop.ObjCellId.GetValueOrDefault());
            writer.Write(prop.OriginX != null);
            if (prop.OriginX != null)
                writer.Write(prop.OriginX.GetValueOrDefault());
            writer.Write(prop.OriginY != null);
            if (prop.OriginY != null)
                writer.Write(prop.OriginY.GetValueOrDefault());
            writer.Write(prop.OriginZ != null);
            if (prop.OriginZ != null)
                writer.Write(prop.OriginZ.GetValueOrDefault());
            writer.Write(prop.AnglesW != null);
            if (prop.AnglesW != null)
                writer.Write(prop.AnglesW.GetValueOrDefault());
            writer.Write(prop.AnglesX != null);
            if (prop.AnglesX != null)
                writer.Write(prop.AnglesX.GetValueOrDefault());
            writer.Write(prop.AnglesY != null);
            if (prop.AnglesY != null)
                writer.Write(prop.AnglesY.GetValueOrDefault());
            writer.Write(prop.AnglesZ != null);
            if (prop.AnglesZ != null)
                writer.Write(prop.AnglesZ.GetValueOrDefault());
        }
    }
    public static void ReadGenerators(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesGenerator is null)
            biota.BiotaPropertiesGenerator = new List<BiotaPropertiesGenerator>();

        var props = biota.BiotaPropertiesGenerator;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesGenerator
            {
                Id = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                Probability = reader.ReadUInt32(),
                WeenieClassId = reader.ReadUInt32(),
                InitCreate = reader.ReadInt32(),
                MaxCreate = reader.ReadInt32(),
                WhenCreate = reader.ReadUInt32(),
                WhereCreate = reader.ReadUInt32(),
                Object = biota,
            };

            if (reader.ReadBoolean())
                value.Delay = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.StackSize = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.PaletteId = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.Shade = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.ObjCellId = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.OriginX = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.OriginY = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.OriginZ = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesW = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesX = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesY = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesZ = reader.ReadSingle();

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesPalette> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Id);
            writer.Write(prop.Length);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Offset);
            writer.Write(prop.SubPaletteId);

            writer.Write(prop.Order != null);
            if (prop.Order != null)
                writer.Write(prop.Order.GetValueOrDefault());
        }
    }
    public static void ReadPalettes(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesPalette is null)
            biota.BiotaPropertiesPalette = new List<BiotaPropertiesPalette>();

        var props = biota.BiotaPropertiesPalette;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesPalette
            {
                Id = reader.ReadUInt32(),
                Length = reader.ReadUInt16(),
                ObjectId = reader.ReadUInt32(),
                Offset = reader.ReadUInt16(),
                SubPaletteId = reader.ReadUInt32(),
                Object = biota,
            };

            if (reader.ReadBoolean())
                value.Order = reader.ReadByte();

            props.Add(value);
        }

    }
    public static void Write(this ICollection<BiotaPropertiesTextureMap> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Id);
            writer.Write(prop.Index);
            writer.Write(prop.NewId);
            writer.Write(prop.ObjectId);
            writer.Write(prop.OldId);

            writer.Write(prop.Order != null);
            if (prop.Order != null)
                writer.Write(prop.Order.GetValueOrDefault());
        }
    }
    public static void ReadTextureMaps(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesTextureMap is null)
            biota.BiotaPropertiesTextureMap = new List<BiotaPropertiesTextureMap>();

        var props = biota.BiotaPropertiesTextureMap;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesTextureMap
            {
                Id = reader.ReadUInt32(),
                Index = reader.ReadByte(),
                NewId = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                OldId = reader.ReadUInt32(),
                Object = biota,
            };

            if (reader.ReadBoolean())
                value.Order = reader.ReadByte();

            props.Add(value);
        }

    }
    //Bodyparts
    public static void Write(this ICollection<BiotaPropertiesBodyPart> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ArmorVsAcid);
            writer.Write(prop.ArmorVsBludgeon);
            writer.Write(prop.ArmorVsCold);
            writer.Write(prop.ArmorVsElectric);
            writer.Write(prop.ArmorVsFire);
            writer.Write(prop.ArmorVsNether);
            writer.Write(prop.ArmorVsPierce);
            writer.Write(prop.ArmorVsSlash);
            writer.Write(prop.BaseArmor);
            writer.Write(prop.BH);
            writer.Write(prop.DType);
            writer.Write(prop.DVal);
            writer.Write(prop.DVar);
            writer.Write(prop.HLB);
            writer.Write(prop.HLF);
            writer.Write(prop.HRB);
            writer.Write(prop.HRF);
            writer.Write(prop.Id);
            writer.Write(prop.Key);
            writer.Write(prop.LLB);
            writer.Write(prop.LLF);
            writer.Write(prop.LRB);
            writer.Write(prop.LRF);
            writer.Write(prop.MLB);
            writer.Write(prop.MLF);
            writer.Write(prop.MRB);
            writer.Write(prop.MRF);
            writer.Write(prop.ObjectId);
        }
    }
    public static void ReadBodyParts(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesBodyPart is null)
            biota.BiotaPropertiesBodyPart = new List<BiotaPropertiesBodyPart>();

        var props = biota.BiotaPropertiesBodyPart;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesBodyPart
            {
                ArmorVsAcid = reader.ReadInt32(),
                ArmorVsBludgeon = reader.ReadInt32(),
                ArmorVsCold = reader.ReadInt32(),
                ArmorVsElectric = reader.ReadInt32(),
                ArmorVsFire = reader.ReadInt32(),
                ArmorVsNether = reader.ReadInt32(),
                ArmorVsPierce = reader.ReadInt32(),
                ArmorVsSlash = reader.ReadInt32(),
                BaseArmor = reader.ReadInt32(),
                BH = reader.ReadInt32(),
                DType = reader.ReadInt32(),
                DVal = reader.ReadInt32(),
                DVar = reader.ReadSingle(),
                HLB = reader.ReadSingle(),
                HLF = reader.ReadSingle(),
                HRB = reader.ReadSingle(),
                HRF = reader.ReadSingle(),
                Id = reader.ReadUInt32(),
                Key = reader.ReadUInt16(),
                LLB = reader.ReadSingle(),
                LLF = reader.ReadSingle(),
                LRB = reader.ReadSingle(),
                LRF = reader.ReadSingle(),
                MLB = reader.ReadSingle(),
                MLF = reader.ReadSingle(),
                MRB = reader.ReadSingle(),
                MRF = reader.ReadSingle(),
                ObjectId = reader.ReadUInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    //Emotes
    public static void Write(this ICollection<BiotaPropertiesEmote> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Category);
            writer.Write(prop.Id);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Probability);

            writer.Write(prop.Quest != null);
            if (prop.Quest != null)
                writer.Write(prop.Quest);

            writer.Write(prop.MaxHealth != null);
            if (prop.MaxHealth != null)
                writer.Write(prop.MaxHealth.GetValueOrDefault());

            writer.Write(prop.MinHealth != null);
            if (prop.MinHealth != null)
                writer.Write(prop.MinHealth.GetValueOrDefault());

            writer.Write(prop.Style != null);
            if (prop.Style != null)
                writer.Write(prop.Style.GetValueOrDefault());

            writer.Write(prop.Substyle != null);
            if (prop.Substyle != null)
                writer.Write(prop.Substyle.GetValueOrDefault());

            writer.Write(prop.VendorType != null);
            if (prop.VendorType != null)
                writer.Write(prop.VendorType.GetValueOrDefault());

            writer.Write(prop.WeenieClassId != null);
            if (prop.WeenieClassId != null)
                writer.Write(prop.WeenieClassId.GetValueOrDefault());

            //Write emote actions for this emote
            prop.BiotaPropertiesEmoteAction.Write(writer);
        }
    }
    public static void ReadEmotes(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesEmote is null)
            biota.BiotaPropertiesEmote = new List<BiotaPropertiesEmote>();

        var props = biota.BiotaPropertiesEmote;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesEmote
            {
                Category = reader.ReadUInt32(),
                Id = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                Probability = reader.ReadSingle(),
                Object = biota,
                BiotaPropertiesEmoteAction = new List<BiotaPropertiesEmoteAction>(),
            };
            if (reader.ReadBoolean())
                value.Quest = reader.ReadString();
            if (reader.ReadBoolean())
                value.MaxHealth = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.MinHealth = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.Style = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.Substyle = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.VendorType = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.WeenieClassId = reader.ReadUInt32();

            //Read emote actions for this emote.  They check themselves for null
            value.ReadEmoteActions(reader);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesEmoteAction> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.Id);
            writer.Write(prop.EmoteId);
            writer.Write(prop.Order);
            writer.Write(prop.Type);
            writer.Write(prop.Delay);
            writer.Write(prop.Extent);


            writer.Write(prop.Message != null);
            if (prop.Message != null)
                writer.Write(prop.Message);
            writer.Write(prop.TestString != null);
            if (prop.TestString != null)
                writer.Write(prop.TestString);
            writer.Write(prop.Motion != null);
            if (prop.Motion != null)
                writer.Write(prop.Motion.GetValueOrDefault());
            writer.Write(prop.Min != null);
            if (prop.Min != null)
                writer.Write(prop.Min.GetValueOrDefault());
            writer.Write(prop.Max != null);
            if (prop.Max != null)
                writer.Write(prop.Max.GetValueOrDefault());
            writer.Write(prop.Min64 != null);
            if (prop.Min64 != null)
                writer.Write(prop.Min64.GetValueOrDefault());
            writer.Write(prop.Max64 != null);
            if (prop.Max64 != null)
                writer.Write(prop.Max64.GetValueOrDefault());
            writer.Write(prop.MinDbl != null);
            if (prop.MinDbl != null)
                writer.Write(prop.MinDbl.GetValueOrDefault());
            writer.Write(prop.MaxDbl != null);
            if (prop.MaxDbl != null)
                writer.Write(prop.MaxDbl.GetValueOrDefault());
            writer.Write(prop.Stat != null);
            if (prop.Stat != null)
                writer.Write(prop.Stat.GetValueOrDefault());
            writer.Write(prop.Display != null);
            if (prop.Display != null)
                writer.Write(prop.Display.GetValueOrDefault());
            writer.Write(prop.Amount != null);
            if (prop.Amount != null)
                writer.Write(prop.Amount.GetValueOrDefault());
            writer.Write(prop.Amount64 != null);
            if (prop.Amount64 != null)
                writer.Write(prop.Amount64.GetValueOrDefault());
            writer.Write(prop.HeroXP64 != null);
            if (prop.HeroXP64 != null)
                writer.Write(prop.HeroXP64.GetValueOrDefault());
            writer.Write(prop.Percent != null);
            if (prop.Percent != null)
                writer.Write(prop.Percent.GetValueOrDefault());
            writer.Write(prop.SpellId != null);
            if (prop.SpellId != null)
                writer.Write(prop.SpellId.GetValueOrDefault());
            writer.Write(prop.WealthRating != null);
            if (prop.WealthRating != null)
                writer.Write(prop.WealthRating.GetValueOrDefault());
            writer.Write(prop.TreasureClass != null);
            if (prop.TreasureClass != null)
                writer.Write(prop.TreasureClass.GetValueOrDefault());
            writer.Write(prop.TreasureType != null);
            if (prop.TreasureType != null)
                writer.Write(prop.TreasureType.GetValueOrDefault());
            writer.Write(prop.PScript != null);
            if (prop.PScript != null)
                writer.Write(prop.PScript.GetValueOrDefault());
            writer.Write(prop.Sound != null);
            if (prop.Sound != null)
                writer.Write(prop.Sound.GetValueOrDefault());
            writer.Write(prop.DestinationType != null);
            if (prop.DestinationType != null)
                writer.Write(prop.DestinationType.GetValueOrDefault());
            writer.Write(prop.WeenieClassId != null);
            if (prop.WeenieClassId != null)
                writer.Write(prop.WeenieClassId.GetValueOrDefault());
            writer.Write(prop.StackSize != null);
            if (prop.StackSize != null)
                writer.Write(prop.StackSize.GetValueOrDefault());
            writer.Write(prop.Palette != null);
            if (prop.Palette != null)
                writer.Write(prop.Palette.GetValueOrDefault());
            writer.Write(prop.Shade != null);
            if (prop.Shade != null)
                writer.Write(prop.Shade.GetValueOrDefault());
            writer.Write(prop.TryToBond != null);
            if (prop.TryToBond != null)
                writer.Write(prop.TryToBond.GetValueOrDefault());
            writer.Write(prop.ObjCellId != null);
            if (prop.ObjCellId != null)
                writer.Write(prop.ObjCellId.GetValueOrDefault());
            writer.Write(prop.OriginX != null);
            if (prop.OriginX != null)
                writer.Write(prop.OriginX.GetValueOrDefault());
            writer.Write(prop.OriginY != null);
            if (prop.OriginY != null)
                writer.Write(prop.OriginY.GetValueOrDefault());
            writer.Write(prop.OriginZ != null);
            if (prop.OriginZ != null)
                writer.Write(prop.OriginZ.GetValueOrDefault());
            writer.Write(prop.AnglesW != null);
            if (prop.AnglesW != null)
                writer.Write(prop.AnglesW.GetValueOrDefault());
            writer.Write(prop.AnglesX != null);
            if (prop.AnglesX != null)
                writer.Write(prop.AnglesX.GetValueOrDefault());
            writer.Write(prop.AnglesY != null);
            if (prop.AnglesY != null)
                writer.Write(prop.AnglesY.GetValueOrDefault());
            writer.Write(prop.AnglesZ != null);
            if (prop.AnglesZ != null)
                writer.Write(prop.AnglesZ.GetValueOrDefault());
        }
    }
    public static void ReadEmoteActions(this BiotaPropertiesEmote emote, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        var props = emote.BiotaPropertiesEmoteAction;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesEmoteAction
            {
                Id = reader.ReadUInt32(),
                EmoteId = reader.ReadUInt32(),
                Order = reader.ReadUInt32(),
                Type = reader.ReadUInt32(),
                Delay = reader.ReadSingle(),
                Extent = reader.ReadSingle(),
                Emote = emote,
            };

            if (reader.ReadBoolean())
                value.Message = reader.ReadString();
            if (reader.ReadBoolean())
                value.TestString = reader.ReadString();
            if (reader.ReadBoolean())
                value.Motion = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.Min = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Max = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Min64 = reader.ReadInt64();
            if (reader.ReadBoolean())
                value.Max64 = reader.ReadInt64();
            if (reader.ReadBoolean())
                value.MinDbl = reader.ReadDouble();
            if (reader.ReadBoolean())
                value.MaxDbl = reader.ReadDouble();
            if (reader.ReadBoolean())
                value.Stat = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Display = reader.ReadBoolean();
            if (reader.ReadBoolean())
                value.Amount = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Amount64 = reader.ReadInt64();
            if (reader.ReadBoolean())
                value.HeroXP64 = reader.ReadInt64();
            if (reader.ReadBoolean())
                value.Percent = reader.ReadDouble();
            if (reader.ReadBoolean())
                value.SpellId = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.WealthRating = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.TreasureClass = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.TreasureType = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.PScript = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Sound = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.DestinationType = reader.ReadSByte();
            if (reader.ReadBoolean())
                value.WeenieClassId = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.StackSize = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Palette = reader.ReadInt32();
            if (reader.ReadBoolean())
                value.Shade = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.TryToBond = reader.ReadBoolean();
            if (reader.ReadBoolean())
                value.ObjCellId = reader.ReadUInt32();
            if (reader.ReadBoolean())
                value.OriginX = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.OriginY = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.OriginZ = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesW = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesX = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesY = reader.ReadSingle();
            if (reader.ReadBoolean())
                value.AnglesZ = reader.ReadSingle();
        }
    }
    //Attributes and Vitals
    public static void Write(this ICollection<BiotaPropertiesAttribute> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.CPSpent);
            writer.Write(prop.InitLevel);
            writer.Write(prop.LevelFromCP);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
        }
    }
    public static void ReadAttributes(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesAttribute is null)
            biota.BiotaPropertiesAttribute = new List<BiotaPropertiesAttribute>();

        var props = biota.BiotaPropertiesAttribute;
        int count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesAttribute
            {
                CPSpent = reader.ReadUInt32(),
                InitLevel = reader.ReadUInt32(),
                LevelFromCP = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesAttribute2nd> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.CPSpent);
            writer.Write(prop.CurrentLevel);
            writer.Write(prop.InitLevel);
            writer.Write(prop.LevelFromCP);
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
        }
    }
    public static void ReadAttribute2nds(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesAttribute2nd is null)
            biota.BiotaPropertiesAttribute2nd = new List<BiotaPropertiesAttribute2nd>();

        var props = biota.BiotaPropertiesAttribute2nd;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesAttribute2nd
            {
                CPSpent = reader.ReadUInt32(),
                CurrentLevel = reader.ReadUInt32(),
                InitLevel = reader.ReadUInt32(),
                LevelFromCP = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    //Positions
    public static void Write(this ICollection<BiotaPropertiesPosition> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjCellId);
            writer.Write(prop.ObjectId);
            writer.Write(prop.PositionType);
            writer.Write(prop.OriginX);
            writer.Write(prop.OriginY);
            writer.Write(prop.OriginZ);
            writer.Write(prop.AnglesX);
            writer.Write(prop.AnglesY);
            writer.Write(prop.AnglesZ);
            writer.Write(prop.AnglesW);
        }
    }
    public static void ReadPositions(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesPosition is null)
            biota.BiotaPropertiesPosition = new List<BiotaPropertiesPosition>();

        var props = biota.BiotaPropertiesPosition;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesPosition
            {
                ObjCellId = reader.ReadUInt32(),
                ObjectId = reader.ReadUInt32(),
                PositionType = reader.ReadUInt16(),
                OriginX = reader.ReadSingle(),
                OriginY = reader.ReadSingle(),
                OriginZ = reader.ReadSingle(),
                AnglesX = reader.ReadSingle(),
                AnglesY = reader.ReadSingle(),
                AnglesZ = reader.ReadSingle(),
                AnglesW = reader.ReadSingle(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    //Skills
    public static void Write(this ICollection<BiotaPropertiesSkill> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.InitLevel);
            writer.Write(prop.LastUsedTime);
            writer.Write(prop.LevelFromPP);
            writer.Write(prop.ObjectId);
            writer.Write(prop.PP);
            writer.Write(prop.ResistanceAtLastCheck);
            writer.Write(prop.SAC);
            writer.Write(prop.Type);
        }
    }
    public static void ReadSkills(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesSkill is null)
            biota.BiotaPropertiesSkill = new List<BiotaPropertiesSkill>();

        var props = biota.BiotaPropertiesSkill;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesSkill
            {
                InitLevel = reader.ReadUInt32(),
                LastUsedTime = reader.ReadDouble(),
                LevelFromPP = reader.ReadUInt16(),
                ObjectId = reader.ReadUInt32(),
                PP = reader.ReadUInt32(),
                ResistanceAtLastCheck = reader.ReadUInt32(),
                SAC = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    //Spellbook
    public static void Write(this ICollection<BiotaPropertiesSpellBook> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Probability);
            writer.Write(prop.Spell);
        }
    }
    public static void ReadSpellbook(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesSpellBook is null)
            biota.BiotaPropertiesSpellBook = new List<BiotaPropertiesSpellBook>();

        var props = biota.BiotaPropertiesSpellBook;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesSpellBook
            {
                ObjectId = reader.ReadUInt32(),
                Probability = reader.ReadSingle(),
                Spell = reader.ReadInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    //Standard
    public static void Write(this ICollection<BiotaPropertiesBool> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadBools(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesBool is null)
            biota.BiotaPropertiesBool = new List<BiotaPropertiesBool>();

        var props = biota.BiotaPropertiesBool;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesBool
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadBoolean(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesDID> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadDIDs(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesDID is null)
            biota.BiotaPropertiesDID = new List<BiotaPropertiesDID>();

        var props = biota.BiotaPropertiesDID;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesDID
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadUInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesFloat> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadFloats(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesFloat is null)
            biota.BiotaPropertiesFloat = new List<BiotaPropertiesFloat>();

        var props = biota.BiotaPropertiesFloat;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesFloat
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadDouble(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesIID> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadIIDs(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesIID is null)
            biota.BiotaPropertiesIID = new List<BiotaPropertiesIID>();

        var props = biota.BiotaPropertiesIID;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesIID
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadUInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesInt> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadInts(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesInt is null)
            biota.BiotaPropertiesInt = new List<BiotaPropertiesInt>();

        var props = biota.BiotaPropertiesInt;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesInt
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadInt32(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesInt64> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadInt64s(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesInt64 is null)
            biota.BiotaPropertiesInt64 = new List<BiotaPropertiesInt64>();

        var props = biota.BiotaPropertiesInt64;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesInt64
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadInt64(),
                Object = biota,
            };

            props.Add(value);
        }
    }
    public static void Write(this ICollection<BiotaPropertiesString> props, BinaryWriter writer)
    {
        writer.Write(props != null);
        if (props is null)
            return;

        writer.Write(props.Count);

        foreach (var prop in props)
        {
            writer.Write(prop.ObjectId);
            writer.Write(prop.Type);
            writer.Write(prop.Value);
        }
    }
    public static void ReadStrings(this Biota biota, BinaryReader reader)
    {
        if (!reader.ReadBoolean())
            return;

        if (biota.BiotaPropertiesString is null)
            biota.BiotaPropertiesString = new List<BiotaPropertiesString>();

        var props = biota.BiotaPropertiesString;
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new BiotaPropertiesString
            {
                ObjectId = reader.ReadUInt32(),
                Type = reader.ReadUInt16(),
                Value = reader.ReadString(),
                Object = biota,
            };

            props.Add(value);
        }
    }


    public static Player CreatePlayer(this ACE.Entity.Models.Biota newPlayerBiota,
    List<Biota> newInventoryItems,
    List<Biota> newWieldedItems,
    Character newCharacter,
    Session session)
    {
        Player newPlayer;
        if (newPlayerBiota.WeenieType == WeenieType.Admin)
            newPlayer = new Admin(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
        else if (newPlayerBiota.WeenieType == WeenieType.Sentinel)
            newPlayer = new Sentinel(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);
        else
            newPlayer = new Player(newPlayerBiota, newInventoryItems, newWieldedItems, newCharacter, session);

        newPlayer.Name = newCharacter.Name;
        newPlayer.ChangesDetected = true;
        newPlayer.CharacterChangesDetected = true;

        newPlayer.Allegiance = null;
        newPlayer.AllegianceOfficerRank = null;
        newPlayer.MonarchId = null;
        newPlayer.PatronId = null;
        newPlayer.HouseId = null;
        newPlayer.HouseInstance = null;

        return newPlayer;
    }

    public static ACE.Entity.Models.Biota CopyBiotaAs(this Biota playerBiota, Character newCharacter, LoadOptions options)
    {
        //Create a new biota for a player
        var newPlayerBiota = ACE.Database.Adapter.BiotaConverter.ConvertToEntityBiota(playerBiota);
        newPlayerBiota.Id = newCharacter.Id;

        //Clear as needed
        newPlayerBiota.PropertiesAllegiance?.Clear();
        newPlayerBiota.HousePermissions?.Clear();

        if (!options.IncludeEnchantments)
            newPlayerBiota.PropertiesEnchantmentRegistry?.Clear();

        if (options.ResetPositions)
        {
            newPlayerBiota.PropertiesPosition = new Dictionary<PositionType, PropertiesPosition>()
            {
                { PositionType.Location, LoadOptions.DefaultPosition },
                { PositionType.Instantiation, LoadOptions.DefaultPosition },
                { PositionType.Sanctuary, LoadOptions.DefaultPosition }
            };
        }

        return newPlayerBiota;
    }

    //public static List<ACE.Database.Models.Shard.Biota> CopyInventoryAs(
    //    this PossessedBiotas existingPossessions,
    //    Character newOwner,
    //    ref ConcurrentDictionary<uint, uint> idSwaps) =>
    //    ChangeOwner(existingPossessions.Inventory, newOwner, ref idSwaps);

    //public static List<ACE.Database.Models.Shard.Biota> CopyWieldedItemsAs(
    //    this PossessedBiotas existingPossessions,
    //    Character newOwner,
    //    ref ConcurrentDictionary<uint, uint> idSwaps) =>
    //    ChangeOwner(existingPossessions.WieldedItems, newOwner, ref idSwaps);
}