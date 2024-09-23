using ACE.Database;
using ACE.Database.Models.World;
using ACE.Database.SQLFormatters.World;
using ACE.Server.Command.Handlers;
using ACE.Server.Command.Handlers.Processors;

namespace ACE.Shared.Helpers;

public static class WeenieExtensions
{
    public static ACE.Database.Models.World.Weenie ToDatabaseWeenie(this ACE.Entity.Models.Weenie weenie)
    {
        ACE.Database.Models.World.Weenie weenie2 = new ACE.Database.Models.World.Weenie();
        weenie2.ClassId = weenie.WeenieClassId;
        weenie2.ClassName = weenie.ClassName;
        weenie2.Type = (int)weenie.WeenieType;
        if (weenie.PropertiesBool != null)
        {
            weenie2.WeeniePropertiesBool = new List<WeeniePropertiesBool>();
            foreach (KeyValuePair<PropertyBool, bool> item2 in weenie.PropertiesBool)
            {
                weenie2.WeeniePropertiesBool.Add(new WeeniePropertiesBool
                {
                    Value = item2.Value,
                    Type = (ushort)item2.Key
                });
            }
        }

        if (weenie.PropertiesDID != null)
        {
            weenie2.WeeniePropertiesDID = new List<WeeniePropertiesDID>();
            foreach (KeyValuePair<PropertyDataId, uint> item3 in weenie.PropertiesDID)
            {
                weenie2.WeeniePropertiesDID.Add(new WeeniePropertiesDID
                {
                    Value = Convert.ToUInt32(item3.Value),
                    Type = (ushort)item3.Key
                });
            }
        }

        if (weenie.PropertiesFloat != null)
        {
            weenie2.WeeniePropertiesFloat = new List<WeeniePropertiesFloat>();
            foreach (KeyValuePair<PropertyFloat, double> item4 in weenie.PropertiesFloat)
            {
                weenie2.WeeniePropertiesFloat.Add(new WeeniePropertiesFloat
                {
                    Value = item4.Value,
                    Type = (ushort)item4.Key
                });
            }
        }

        if (weenie.PropertiesIID != null)
        {
            weenie2.WeeniePropertiesIID = new List<WeeniePropertiesIID>();
#if REALM
            foreach (KeyValuePair<PropertyInstanceId, ulong> item5 in weenie.PropertiesIID)
            {
                weenie2.WeeniePropertiesIID.Add(new WeeniePropertiesIID
                {
                    //Todo: Properly support ACRealms?
                    Value = (uint)item5.Value,
                    Type = (ushort)item5.Key
                });
            }
#else
            foreach (KeyValuePair<PropertyInstanceId, uint> item5 in weenie.PropertiesIID)
            {
                weenie2.WeeniePropertiesIID.Add(new WeeniePropertiesIID
                {
                    //Todo: Properly support ACRealms?
                    Value = (uint)item5.Value,
                    Type = (ushort)item5.Key
                });
            }
#endif
        }

        if (weenie.PropertiesInt != null)
        {
            weenie2.WeeniePropertiesInt = new List<WeeniePropertiesInt>();
            foreach (KeyValuePair<PropertyInt, int> item6 in weenie.PropertiesInt)
            {
                weenie2.WeeniePropertiesInt.Add(new WeeniePropertiesInt
                {
                    Value = item6.Value,
                    Type = (ushort)item6.Key
                });
            }
        }

        if (weenie.PropertiesInt64 != null)
        {
            weenie2.WeeniePropertiesInt64 = new List<WeeniePropertiesInt64>();
            foreach (KeyValuePair<PropertyInt64, long> item7 in weenie.PropertiesInt64)
            {
                weenie2.WeeniePropertiesInt64.Add(new WeeniePropertiesInt64
                {
                    Value = item7.Value,
                    Type = (ushort)item7.Key
                });
            }
        }

        if (weenie.PropertiesInt64 != null)
        {
            weenie2.WeeniePropertiesInt64 = new List<WeeniePropertiesInt64>();
            foreach (KeyValuePair<PropertyInt64, long> item8 in weenie.PropertiesInt64)
            {
                weenie2.WeeniePropertiesInt64.Add(new WeeniePropertiesInt64
                {
                    Value = item8.Value,
                    Type = (ushort)item8.Key
                });
            }
        }

        if (weenie.PropertiesString != null)
        {
            weenie2.WeeniePropertiesString = new List<WeeniePropertiesString>();
            foreach (KeyValuePair<PropertyString, string> item9 in weenie.PropertiesString)
            {
                weenie2.WeeniePropertiesString.Add(new WeeniePropertiesString
                {
                    Value = item9.Value,
                    Type = (ushort)item9.Key
                });
            }
        }

        if (weenie.PropertiesPosition != null && weenie.PropertiesPosition.Count > 0)
        {
            weenie2.WeeniePropertiesPosition = new List<WeeniePropertiesPosition>();
            foreach (KeyValuePair<PositionType, PropertiesPosition> item10 in weenie.PropertiesPosition)
            {
                weenie2.WeeniePropertiesPosition.Add(new WeeniePropertiesPosition
                {
                    PositionType = (ushort)item10.Key,
                    ObjCellId = item10.Value.ObjCellId,
                    OriginX = item10.Value.PositionX,
                    OriginY = item10.Value.PositionY,
                    OriginZ = item10.Value.PositionZ,
                    AnglesW = item10.Value.RotationW,
                    AnglesX = item10.Value.RotationX,
                    AnglesY = item10.Value.RotationY,
                    AnglesZ = item10.Value.RotationZ
                });
            }
        }

        if (weenie.PropertiesSpellBook != null)
        {
            weenie2.WeeniePropertiesSpellBook = new List<WeeniePropertiesSpellBook>();
            foreach (KeyValuePair<int, float> item11 in weenie.PropertiesSpellBook)
            {
                weenie2.WeeniePropertiesSpellBook.Add(new WeeniePropertiesSpellBook
                {
                    Spell = item11.Key,
                    Probability = item11.Value
                });
            }
        }

        if (weenie.PropertiesAnimPart != null)
        {
            weenie2.WeeniePropertiesAnimPart = new List<WeeniePropertiesAnimPart>();
            foreach (PropertiesAnimPart item12 in weenie.PropertiesAnimPart)
            {
                weenie2.WeeniePropertiesAnimPart.Add(new WeeniePropertiesAnimPart
                {
                    Index = item12.Index,
                    AnimationId = item12.AnimationId
                });
            }
        }

        if (weenie.PropertiesPalette != null)
        {
            weenie2.WeeniePropertiesPalette = new List<WeeniePropertiesPalette>();
            foreach (PropertiesPalette item13 in weenie.PropertiesPalette)
            {
                weenie2.WeeniePropertiesPalette.Add(new WeeniePropertiesPalette
                {
                    SubPaletteId = item13.SubPaletteId,
                    Offset = item13.Offset,
                    Length = item13.Length
                });
            }
        }

        if (weenie.PropertiesTextureMap != null)
        {
            weenie2.WeeniePropertiesTextureMap = new List<WeeniePropertiesTextureMap>();
            foreach (PropertiesTextureMap item14 in weenie.PropertiesTextureMap)
            {
                weenie2.WeeniePropertiesTextureMap.Add(new WeeniePropertiesTextureMap
                {
                    Index = item14.PartIndex,
                    OldId = item14.OldTexture,
                    NewId = item14.NewTexture
                });
            }
        }

        if (weenie.PropertiesCreateList != null)
        {
            weenie2.WeeniePropertiesCreateList = new List<WeeniePropertiesCreateList>();
            foreach (PropertiesCreateList propertiesCreate in weenie.PropertiesCreateList)
            {
                weenie2.WeeniePropertiesCreateList.Add(new WeeniePropertiesCreateList
                {
                    DestinationType = (sbyte)propertiesCreate.DestinationType,
                    WeenieClassId = propertiesCreate.WeenieClassId,
                    StackSize = propertiesCreate.StackSize,
                    Palette = propertiesCreate.Palette,
                    Shade = propertiesCreate.Shade,
                    TryToBond = propertiesCreate.TryToBond
                });
            }
        }

        if (weenie.PropertiesEmote != null)
        {
            weenie2.WeeniePropertiesEmote = new List<WeeniePropertiesEmote>();
            foreach (PropertiesEmote item15 in weenie.PropertiesEmote)
            {
                WeeniePropertiesEmote weeniePropertiesEmote = new WeeniePropertiesEmote
                {
                    Category = (uint)item15.Category,
                    Probability = item15.Probability,
                    WeenieClassId = item15.WeenieClassId,
                    Style = (uint?)item15.Style,
                    Substyle = (uint?)item15.Substyle,
                    Quest = item15.Quest,
                    VendorType = (int?)item15.VendorType,
                    MinHealth = item15.MinHealth,
                    MaxHealth = item15.MaxHealth
                };
                uint num = 0u;
                foreach (PropertiesEmoteAction item16 in item15.PropertiesEmoteAction)
                {
                    WeeniePropertiesEmoteAction item = new WeeniePropertiesEmoteAction
                    {
                        Order = num++,
                        Type = item16.Type,
                        Delay = item16.Delay,
                        Extent = item16.Extent,
                        Motion = (uint?)item16.Motion,
                        Message = item16.Message,
                        TestString = item16.TestString,
                        Min = item16.Min,
                        Max = item16.Max,
                        Min64 = item16.Min64,
                        Max64 = item16.Max64,
                        MinDbl = item16.MinDbl,
                        MaxDbl = item16.MaxDbl,
                        Stat = item16.Stat,
                        Display = item16.Display,
                        Amount = item16.Amount,
                        Amount64 = item16.Amount64,
                        HeroXP64 = item16.HeroXP64,
                        Percent = item16.Percent,
                        SpellId = item16.SpellId,
                        WealthRating = item16.WealthRating,
                        TreasureClass = item16.TreasureClass,
                        TreasureType = item16.TreasureType,
                        PScript = (int?)item16.PScript,
                        Sound = (int?)item16.Sound,
                        DestinationType = item16.DestinationType,
                        WeenieClassId = item16.WeenieClassId,
                        StackSize = item16.StackSize,
                        Palette = item16.Palette,
                        Shade = item16.Shade,
                        TryToBond = item16.TryToBond,
                        ObjCellId = item16.ObjCellId,
                        OriginX = item16.OriginX,
                        OriginY = item16.OriginY,
                        OriginZ = item16.OriginZ,
                        AnglesW = item16.AnglesW,
                        AnglesX = item16.AnglesX,
                        AnglesY = item16.AnglesY,
                        AnglesZ = item16.AnglesZ
                    };
                    weeniePropertiesEmote.WeeniePropertiesEmoteAction.Add(item);
                }

                weenie2.WeeniePropertiesEmote.Add(weeniePropertiesEmote);
            }
        }

        if (weenie.PropertiesEventFilter != null)
        {
            weenie2.WeeniePropertiesEventFilter = new List<WeeniePropertiesEventFilter>();
            foreach (int item17 in weenie.PropertiesEventFilter)
            {
                weenie2.WeeniePropertiesEventFilter.Add(new WeeniePropertiesEventFilter
                {
                    Event = item17
                });
            }
        }

        if (weenie.PropertiesGenerator != null)
        {
            weenie2.WeeniePropertiesGenerator = new List<WeeniePropertiesGenerator>();
            foreach (PropertiesGenerator item18 in weenie.PropertiesGenerator)
            {
                weenie2.WeeniePropertiesGenerator.Add(new WeeniePropertiesGenerator
                {
                    Probability = item18.Probability,
                    WeenieClassId = item18.WeenieClassId,
                    Delay = item18.Delay,
                    InitCreate = item18.InitCreate,
                    MaxCreate = item18.MaxCreate,
                    WhenCreate = (uint)item18.WhenCreate,
                    WhereCreate = (uint)item18.WhereCreate,
                    StackSize = item18.StackSize,
                    PaletteId = item18.PaletteId,
                    Shade = item18.Shade,
                    ObjCellId = item18.ObjCellId,
                    OriginX = item18.OriginX,
                    OriginY = item18.OriginY,
                    OriginZ = item18.OriginZ,
                    AnglesW = item18.AnglesW,
                    AnglesX = item18.AnglesX,
                    AnglesY = item18.AnglesY,
                    AnglesZ = item18.AnglesZ
                });
            }
        }

        if (weenie.PropertiesAttribute != null)
        {
            weenie2.WeeniePropertiesAttribute = new List<WeeniePropertiesAttribute>();
            foreach (KeyValuePair<PropertyAttribute, PropertiesAttribute> item19 in weenie.PropertiesAttribute)
            {
                weenie2.WeeniePropertiesAttribute.Add(new WeeniePropertiesAttribute
                {
                    Type = (ushort)item19.Key,
                    InitLevel = item19.Value.InitLevel,
                    LevelFromCP = item19.Value.LevelFromCP,
                    CPSpent = item19.Value.CPSpent
                });
            }
        }

        if (weenie.PropertiesAttribute2nd != null)
        {
            weenie2.WeeniePropertiesAttribute2nd = new List<WeeniePropertiesAttribute2nd>();
            foreach (KeyValuePair<PropertyAttribute2nd, PropertiesAttribute2nd> item20 in weenie.PropertiesAttribute2nd)
            {
                weenie2.WeeniePropertiesAttribute2nd.Add(new WeeniePropertiesAttribute2nd
                {
                    Type = (ushort)item20.Key,
                    InitLevel = item20.Value.InitLevel,
                    LevelFromCP = item20.Value.LevelFromCP,
                    CPSpent = item20.Value.CPSpent
                });
            }
        }

        if (weenie.PropertiesBodyPart != null)
        {
            weenie2.WeeniePropertiesBodyPart = new List<WeeniePropertiesBodyPart>();
            foreach (KeyValuePair<CombatBodyPart, PropertiesBodyPart> item21 in weenie.PropertiesBodyPart)
            {
                weenie2.WeeniePropertiesBodyPart.Add(new WeeniePropertiesBodyPart
                {
                    Key = (ushort)item21.Key,
                    DType = (int)item21.Value.DType,
                    DVal = item21.Value.DVal,
                    DVar = item21.Value.DVar,
                    BaseArmor = item21.Value.BaseArmor,
                    ArmorVsSlash = item21.Value.ArmorVsSlash,
                    ArmorVsPierce = item21.Value.ArmorVsPierce,
                    ArmorVsBludgeon = item21.Value.ArmorVsBludgeon,
                    ArmorVsCold = item21.Value.ArmorVsCold,
                    ArmorVsFire = item21.Value.ArmorVsFire,
                    ArmorVsAcid = item21.Value.ArmorVsAcid,
                    ArmorVsElectric = item21.Value.ArmorVsElectric,
                    ArmorVsNether = item21.Value.ArmorVsNether,
                    BH = item21.Value.BH,
                    HLF = item21.Value.HLF,
                    MLF = item21.Value.MLF,
                    LLF = item21.Value.LLF,
                    HRF = item21.Value.HRF,
                    MRF = item21.Value.MRF,
                    LRF = item21.Value.LRF,
                    HLB = item21.Value.HLB,
                    MLB = item21.Value.MLB,
                    LLB = item21.Value.LLB,
                    HRB = item21.Value.HRB,
                    MRB = item21.Value.MRB,
                    LRB = item21.Value.LRB
                });
            }
        }

        if (weenie.PropertiesSkill != null)
        {
            weenie2.WeeniePropertiesSkill = new List<WeeniePropertiesSkill>();
            foreach (KeyValuePair<Skill, PropertiesSkill> item22 in weenie.PropertiesSkill)
            {
                weenie2.WeeniePropertiesSkill.Add(new WeeniePropertiesSkill
                {
                    Type = (ushort)item22.Key,
                    LevelFromPP = item22.Value.LevelFromPP,
                    SAC = (uint)item22.Value.SAC,
                    PP = item22.Value.PP,
                    InitLevel = item22.Value.InitLevel,
                    ResistanceAtLastCheck = item22.Value.ResistanceAtLastCheck,
                    LastUsedTime = item22.Value.LastUsedTime
                });
            }
        }

        if (weenie.PropertiesBook != null)
        {
            weenie2.WeeniePropertiesBook = new WeeniePropertiesBook
            {
                MaxNumCharsPerPage = weenie.PropertiesBook.MaxNumCharsPerPage,
                MaxNumPages = weenie.PropertiesBook.MaxNumPages
            };
        }

        if (weenie.PropertiesBookPageData != null)
        {
            weenie2.WeeniePropertiesBookPageData = new List<WeeniePropertiesBookPageData>();
            uint num2 = 0u;
            foreach (PropertiesBookPageData propertiesBookPageDatum in weenie.PropertiesBookPageData)
            {
                weenie2.WeeniePropertiesBookPageData.Add(new WeeniePropertiesBookPageData
                {
                    PageId = num2++,
                    AuthorId = (uint)propertiesBookPageDatum.AuthorId,
                    AuthorName = propertiesBookPageDatum.AuthorName,
                    AuthorAccount = propertiesBookPageDatum.AuthorAccount,
                    IgnoreAuthor = propertiesBookPageDatum.IgnoreAuthor,
                    PageText = propertiesBookPageDatum.PageText
                });
            }
        }

        return weenie2;
    }

    public static void ExportSQLWeenie(ACE.Database.Models.World.Weenie weenie, Session session, bool withFolders = false)
    {
        DirectoryInfo directoryInfo = DeveloperContentCommands.VerifyContentFolder(session, false);
        WeenieSQLWriter weenieSQLWriter = new WeenieSQLWriter();
        char directorySeparatorChar = Path.DirectorySeparatorChar;
        string text = null;
        if (withFolders)
        {
            WeenieType type = (WeenieType)weenie.Type;
            if (type == WeenieType.Creature)
            {
                WeeniePropertiesInt weeniePropertiesInt = weenie.WeeniePropertiesInt.Where((WeeniePropertiesInt x) => x.Type == 2).FirstOrDefault();
                if (weeniePropertiesInt == null)
                {
                    text = $"{directoryInfo.FullName}{directorySeparatorChar}sql{directorySeparatorChar}weenies{directorySeparatorChar}{type}{directorySeparatorChar}";
                }
                else
                {
                    CreatureType value = (CreatureType)weeniePropertiesInt.Value;
                    text = $"{directoryInfo.FullName}{directorySeparatorChar}sql{directorySeparatorChar}weenies{directorySeparatorChar}{type}{directorySeparatorChar}{value}{directorySeparatorChar}";
                }
            }
            else
            {
                WeeniePropertiesInt weeniePropertiesInt2 = weenie.WeeniePropertiesInt.Where((WeeniePropertiesInt x) => x.Type == 1).FirstOrDefault();
                if (weeniePropertiesInt2 == null)
                {
                    text = $"{directoryInfo.FullName}{directorySeparatorChar}sql{directorySeparatorChar}weenies{directorySeparatorChar}{type}{directorySeparatorChar}";
                }
                else
                {
                    ItemType value2 = (ItemType)weeniePropertiesInt2.Value;
                    text = $"{directoryInfo.FullName}{directorySeparatorChar}sql{directorySeparatorChar}weenies{directorySeparatorChar}{type}{directorySeparatorChar}{value2}{directorySeparatorChar}";
                }
            }
        }
        else
        {
            text = $"{directoryInfo.FullName}{directorySeparatorChar}sql{directorySeparatorChar}weenies{directorySeparatorChar}";
        }

        directoryInfo = new DirectoryInfo(text);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        if (weenieSQLWriter == null)
        {
            weenieSQLWriter = new WeenieSQLWriter();
            weenieSQLWriter.WeenieNames = DatabaseManager.World.GetAllWeenieNames();
            weenieSQLWriter.SpellNames = DatabaseManager.World.GetAllSpellNames();
            weenieSQLWriter.TreasureDeath = DatabaseManager.World.GetAllTreasureDeath();
            weenieSQLWriter.TreasureWielded = DatabaseManager.World.GetAllTreasureWielded();
            weenieSQLWriter.PacketOpCodes = PacketOpCodeNames.Values;
        }

        string defaultFileName = weenieSQLWriter.GetDefaultFileName(weenie);
        StreamWriter streamWriter = new StreamWriter(text + defaultFileName);
        try
        {
            weenieSQLWriter.CreateSQLDELETEStatement(weenie, streamWriter);
            streamWriter.WriteLine();
            weenieSQLWriter.CreateSQLINSERTStatement(weenie, streamWriter);
            streamWriter.Close();
        }
        catch (Exception value3)
        {
            Console.WriteLine(value3);
            CommandHandlerHelper.WriteOutputInfo(session, "Failed to export " + text + defaultFileName, ChatMessageType.Broadcast);
            return;
        }

        CommandHandlerHelper.WriteOutputInfo(session, "Exported " + text + defaultFileName, ChatMessageType.Broadcast);
    }
    public static bool IsNpc(this ACE.Entity.Models.Weenie weenie)
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
