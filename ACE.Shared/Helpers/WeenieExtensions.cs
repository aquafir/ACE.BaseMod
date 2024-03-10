using ACE.Database.Models.World;
using Weenie = ACE.Entity.Models.Weenie;

namespace ACE.Shared.Helpers;

public static class WeenieExtensions
{
    /// <summary>
    /// Converts an Entity Weenie to a Database Weeenie
    /// </summary>
    public static ACE.Database.Models.World.Weenie ToDatabaseWeenie(this ACE.Entity.Models.Weenie weenie)
    {
        var result = new ACE.Database.Models.World.Weenie();

        result.ClassId = weenie.WeenieClassId;
        result.ClassName = weenie.ClassName;
        result.Type = (int)weenie.WeenieType;

        if (weenie.PropertiesBool != null)
        {
            result.WeeniePropertiesBool = new List<WeeniePropertiesBool>();
            foreach (var value in weenie.PropertiesBool)
            {
                result.WeeniePropertiesBool.Add(new WeeniePropertiesBool { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesDID != null)
        {
            result.WeeniePropertiesDID = new List<WeeniePropertiesDID>();
            foreach (var value in weenie.PropertiesDID)
            {

                result.WeeniePropertiesDID.Add(new WeeniePropertiesDID { Value = Convert.ToUInt32(value.Value), Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesFloat != null)
        {
            result.WeeniePropertiesFloat = new List<WeeniePropertiesFloat>();
            foreach (var value in weenie.PropertiesFloat)
            {

                result.WeeniePropertiesFloat.Add(new WeeniePropertiesFloat { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesIID != null)
        {
            result.WeeniePropertiesIID = new List<WeeniePropertiesIID>();
            foreach (var value in weenie.PropertiesIID)
            {

                result.WeeniePropertiesIID.Add(new WeeniePropertiesIID { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesInt != null)
        {
            result.WeeniePropertiesInt = new List<WeeniePropertiesInt>();
            foreach (var value in weenie.PropertiesInt)
            {

                result.WeeniePropertiesInt.Add(new WeeniePropertiesInt { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesInt64 != null)
        {
            result.WeeniePropertiesInt64 = new List<WeeniePropertiesInt64>();
            foreach (var value in weenie.PropertiesInt64)
            {

                result.WeeniePropertiesInt64.Add(new WeeniePropertiesInt64 { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesInt64 != null)
        {
            result.WeeniePropertiesInt64 = new List<WeeniePropertiesInt64>();
            foreach (var value in weenie.PropertiesInt64)
            {

                result.WeeniePropertiesInt64.Add(new WeeniePropertiesInt64 { Value = value.Value, Type = (ushort)value.Key });
            }
        }
        if (weenie.PropertiesString != null)
        {
            result.WeeniePropertiesString = new List<WeeniePropertiesString>();
            foreach (var value in weenie.PropertiesString)
            {

                result.WeeniePropertiesString.Add(new WeeniePropertiesString { Value = value.Value, Type = (ushort)value.Key });
            }
        }

        //TODO: Verify some of these less simple ones properties
        if (weenie.PropertiesPosition != null && weenie.PropertiesPosition.Count > 0)
        {
            result.WeeniePropertiesPosition = new List<WeeniePropertiesPosition>();
            foreach (var value in weenie.PropertiesPosition)
            {
                result.WeeniePropertiesPosition.Add(
                    new WeeniePropertiesPosition
                    {
                        PositionType = (ushort)value.Key,
                        ObjCellId = value.Value.ObjCellId,
                        OriginX = value.Value.PositionX,
                        OriginY = value.Value.PositionY,
                        OriginZ = value.Value.PositionZ,
                        AnglesW = value.Value.RotationW,
                        AnglesX = value.Value.RotationX,
                        AnglesY = value.Value.RotationY,
                        AnglesZ = value.Value.RotationZ
                    }
                );
            }
        }

        if (weenie.PropertiesSpellBook != null)
        {
            result.WeeniePropertiesSpellBook = new List<WeeniePropertiesSpellBook>();
            foreach (var value in weenie.PropertiesSpellBook)
            {
                result.WeeniePropertiesSpellBook.Add(
                    new WeeniePropertiesSpellBook
                    {
                        Spell = value.Key,
                        Probability = value.Value
                    });
            }
        }

        if (weenie.PropertiesAnimPart != null)
        {
            result.WeeniePropertiesAnimPart = new List<WeeniePropertiesAnimPart>();
            foreach (var value in weenie.PropertiesAnimPart)
            {
                result.WeeniePropertiesAnimPart.Add(
                    new WeeniePropertiesAnimPart
                    {
                        Index = value.Index,
                        AnimationId = value.AnimationId
                    });
            }
        }

        if (weenie.PropertiesPalette != null)
        {
            result.WeeniePropertiesPalette = new List<WeeniePropertiesPalette>();
            foreach (var value in weenie.PropertiesPalette)
            {
                result.WeeniePropertiesPalette.Add(
                    new WeeniePropertiesPalette
                    {
                        SubPaletteId = value.SubPaletteId,
                        Offset = value.Offset,
                        Length = value.Length
                    });
            }
        }

        if (weenie.PropertiesTextureMap != null)
        {
            result.WeeniePropertiesTextureMap = new List<WeeniePropertiesTextureMap>();
            foreach (var value in weenie.PropertiesTextureMap)
            {
                result.WeeniePropertiesTextureMap.Add(
                    new WeeniePropertiesTextureMap
                    {
                        Index = value.PartIndex,
                        OldId = value.OldTexture,
                        NewId = value.NewTexture
                    });
            }
        }

        // Properties for all world objects that typically aren't modified over the original weenie
        if (weenie.PropertiesCreateList != null)
        {
            result.WeeniePropertiesCreateList = new List<WeeniePropertiesCreateList>();
            foreach (var value in weenie.PropertiesCreateList)
            {
                result.WeeniePropertiesCreateList.Add(
                    new WeeniePropertiesCreateList
                    {
                        DestinationType = (sbyte)value.DestinationType,
                        WeenieClassId = value.WeenieClassId,
                        StackSize = value.StackSize,
                        Palette = value.Palette,
                        Shade = value.Shade,
                        TryToBond = value.TryToBond
                    });
            }
        }

        if (weenie.PropertiesEmote != null)
        {
            result.WeeniePropertiesEmote = new List<WeeniePropertiesEmote>();
            foreach (var value in weenie.PropertiesEmote)
            {
                var entity1 = new WeeniePropertiesEmote
                {
                    Category = (uint)value.Category,
                    Probability = value.Probability,
                    WeenieClassId = value.WeenieClassId,
                    Style = (uint?)value.Style,
                    Substyle = (uint?)value.Substyle,
                    Quest = value.Quest,
                    VendorType = (int?)value.VendorType,
                    MinHealth = value.MinHealth,
                    MaxHealth = value.MaxHealth
                };

                //Assume correct order and keep count to preserve?
                uint order = 0;
                foreach (var record2 in value.PropertiesEmoteAction)
                {
                    var entity2 = new WeeniePropertiesEmoteAction()
                    {
                        Order = order++,
                        Type = (uint)record2.Type,
                        Delay = record2.Delay,
                        Extent = record2.Extent,
                        Motion = (uint?)record2.Motion,
                        Message = record2.Message,
                        TestString = record2.TestString,
                        Min = record2.Min,
                        Max = record2.Max,
                        Min64 = record2.Min64,
                        Max64 = record2.Max64,
                        MinDbl = record2.MinDbl,
                        MaxDbl = record2.MaxDbl,
                        Stat = record2.Stat,
                        Display = record2.Display,
                        Amount = record2.Amount,
                        Amount64 = record2.Amount64,
                        HeroXP64 = record2.HeroXP64,
                        Percent = record2.Percent,
                        SpellId = record2.SpellId,
                        WealthRating = record2.WealthRating,
                        TreasureClass = record2.TreasureClass,
                        TreasureType = record2.TreasureType,
                        PScript = (int?)record2.PScript,
                        Sound = (int?)record2.Sound,
                        DestinationType = record2.DestinationType,
                        WeenieClassId = record2.WeenieClassId,
                        StackSize = record2.StackSize,
                        Palette = record2.Palette,
                        Shade = record2.Shade,
                        TryToBond = record2.TryToBond,
                        ObjCellId = record2.ObjCellId,
                        OriginX = record2.OriginX,
                        OriginY = record2.OriginY,
                        OriginZ = record2.OriginZ,
                        AnglesW = record2.AnglesW,
                        AnglesX = record2.AnglesX,
                        AnglesY = record2.AnglesY,
                        AnglesZ = record2.AnglesZ,
                    };
                    entity1.WeeniePropertiesEmoteAction.Add(entity2);
                }

                result.WeeniePropertiesEmote.Add(entity1);
            }
        }

        if (weenie.PropertiesEventFilter != null)
        {
            result.WeeniePropertiesEventFilter = new List<WeeniePropertiesEventFilter>();
            foreach (var value in weenie.PropertiesEventFilter)
            {
                result.WeeniePropertiesEventFilter.Add(
                    new WeeniePropertiesEventFilter
                    {
                        Event = value
                    });
            }
        }

        if (weenie.PropertiesGenerator != null)
        {
            result.WeeniePropertiesGenerator = new List<WeeniePropertiesGenerator>();
            foreach (var value in weenie.PropertiesGenerator)
            {
                result.WeeniePropertiesGenerator.Add(
                    new WeeniePropertiesGenerator
                    {
                        Probability = value.Probability,
                        WeenieClassId = value.WeenieClassId,
                        Delay = value.Delay,
                        InitCreate = value.InitCreate,
                        MaxCreate = value.MaxCreate,
                        WhenCreate = (uint)value.WhenCreate,
                        WhereCreate = (uint)value.WhereCreate,
                        StackSize = value.StackSize,
                        PaletteId = value.PaletteId,
                        Shade = value.Shade,
                        ObjCellId = value.ObjCellId,
                        OriginX = value.OriginX,
                        OriginY = value.OriginY,
                        OriginZ = value.OriginZ,
                        AnglesW = value.AnglesW,
                        AnglesX = value.AnglesX,
                        AnglesY = value.AnglesY,
                        AnglesZ = value.AnglesZ
                    });
            }
        }

        // Properties for creatures
        if (weenie.PropertiesAttribute != null)
        {
            result.WeeniePropertiesAttribute = new List<WeeniePropertiesAttribute>();
            foreach (var value in weenie.PropertiesAttribute)
            {
                result.WeeniePropertiesAttribute.Add(
                    new WeeniePropertiesAttribute
                    {
                        Type = (ushort)value.Key,
                        InitLevel = value.Value.InitLevel,
                        LevelFromCP = value.Value.LevelFromCP,
                        CPSpent = value.Value.CPSpent
                    });
            }
        }

        if (weenie.PropertiesAttribute2nd != null)
        {
            result.WeeniePropertiesAttribute2nd = new List<WeeniePropertiesAttribute2nd>();
            foreach (var value in weenie.PropertiesAttribute2nd)
            {
                result.WeeniePropertiesAttribute2nd.Add(
                    new WeeniePropertiesAttribute2nd
                    {
                        Type = (ushort)value.Key,
                        InitLevel = value.Value.InitLevel,
                        LevelFromCP = value.Value.LevelFromCP,
                        CPSpent = value.Value.CPSpent
                    });
            }
        }

        if (weenie.PropertiesBodyPart != null)
        {
            result.WeeniePropertiesBodyPart = new List<WeeniePropertiesBodyPart>();
            foreach (var value in weenie.PropertiesBodyPart)
            {
                result.WeeniePropertiesBodyPart.Add(
                    new WeeniePropertiesBodyPart
                    {
                        Key = (ushort)value.Key,
                        DType = (int)value.Value.DType,
                        DVal = value.Value.DVal,
                        DVar = value.Value.DVar,
                        BaseArmor = value.Value.BaseArmor,
                        ArmorVsSlash = value.Value.ArmorVsSlash,
                        ArmorVsPierce = value.Value.ArmorVsPierce,
                        ArmorVsBludgeon = value.Value.ArmorVsBludgeon,
                        ArmorVsCold = value.Value.ArmorVsCold,
                        ArmorVsFire = value.Value.ArmorVsFire,
                        ArmorVsAcid = value.Value.ArmorVsAcid,
                        ArmorVsElectric = value.Value.ArmorVsElectric,
                        ArmorVsNether = value.Value.ArmorVsNether,
                        BH = value.Value.BH,
                        HLF = value.Value.HLF,
                        MLF = value.Value.MLF,
                        LLF = value.Value.LLF,
                        HRF = value.Value.HRF,
                        MRF = value.Value.MRF,
                        LRF = value.Value.LRF,
                        HLB = value.Value.HLB,
                        MLB = value.Value.MLB,
                        LLB = value.Value.LLB,
                        HRB = value.Value.HRB,
                        MRB = value.Value.MRB,
                        LRB = value.Value.LRB
                    });
            }
        }

        if (weenie.PropertiesSkill != null)
        {
            result.WeeniePropertiesSkill = new List<WeeniePropertiesSkill>();
            foreach (var value in weenie.PropertiesSkill)
            {
                result.WeeniePropertiesSkill.Add(
                    new WeeniePropertiesSkill
                    {
                        Type = (ushort)value.Key,
                        LevelFromPP = value.Value.LevelFromPP,
                        SAC = (uint)value.Value.SAC,
                        PP = value.Value.PP,
                        InitLevel = value.Value.InitLevel,
                        ResistanceAtLastCheck = value.Value.ResistanceAtLastCheck,
                        LastUsedTime = value.Value.LastUsedTime,
                    });
            }
        }

        // Properties for books
        if (weenie.PropertiesBook != null)
        {
            result.WeeniePropertiesBook = new WeeniePropertiesBook
            {
                MaxNumCharsPerPage = weenie.PropertiesBook.MaxNumCharsPerPage,
                MaxNumPages = weenie.PropertiesBook.MaxNumPages
            };
        }

        if (weenie.PropertiesBookPageData != null)
        {
            result.WeeniePropertiesBookPageData = new List<WeeniePropertiesBookPageData>();

            //TODO: Assume page ID is correct and starts at 0?
            uint pageId = 0;
            foreach (var value in weenie.PropertiesBookPageData)
            {
                result.WeeniePropertiesBookPageData.Add(
                    new WeeniePropertiesBookPageData
                    {
                        PageId = pageId++,
                        AuthorId = value.AuthorId,
                        AuthorName = value.AuthorName,
                        AuthorAccount = value.AuthorAccount,
                        IgnoreAuthor = value.IgnoreAuthor,
                        PageText = value.PageText,
                    });
            }
        }

        return result;
    }


    public static bool IsNpc(this Weenie weenie)
    {
        //Assume npc
        if (weenie is null) return true;

        //Check NPC as no target, unattackable
        var target = weenie.GetProperty(PropertyInt.TargetingTactic);
        if (target is null || target != (int)ACE.Entity.Enum.TargetingTactic.None)
            return false;

        if (weenie.GetProperty(PropertyBool.Attackable) ?? true)
            return false;

        return true;
    }
}
