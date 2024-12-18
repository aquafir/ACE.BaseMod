﻿///////////////////////////////////////////////////////////////////////////////
//File: Constants.cs
//
//Description: Contains constant data and enums relating to game properties.
//  This file is shared between the VTClassic Plugin and the VTClassic Editor.
//
//This file is Copyright (c) 2009-2010 VirindiPlugins
//
//The original copy of this code can be obtained from http://www.virindi.net/repos/virindi_public
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////


#if VTC_PLUGIN
#endif

namespace AutoLoot.Lib.VTClassic
{
#if VTC_EDITOR
    public enum PropertyString
    {
        Name = 1,
        Title = 5,
        Inscription = 7,
        InscribedBy = 8,
        FellowshipName = 10,
        UsageInstructions = 14,
        SimpleDescription = 15,
        FullDescription = 16,
        MonarchName = 21,
        OnlyActivatedBy = 25,
        Patron = 35,
        PortalDestination = 38,
        LastTinkeredBy = 39,
        ImbuedBy = 40,
        DateBorn = 43,
        SecondaryName = 184549376,
    }
    public enum PropertyInt
    {
        Species = 2,
        Burden = 5,
        EquippedSlots = 10,
        RareId = 17,
        Value = 19,
        TotalValue = 20,
        SkillCreditsAvail = 24,
        CreatureLevel = 25,
        RestrictedToToD = 26,
        ArmorLevel = 28,
        Rank = 30,
        Bonded = 33,
        NumberFollowers = 35,
        Unenchantable = 36,
        LockpickDifficulty = 38,
        Deaths = 43,
        WandElemDmgType = 45,
        MinLevelRestrict = 86,
        MaxLevelRestrict = 87,
        LockpickSkillBonus = 88,
        AffectsVitalId = 89,
        AffectsVitalAmt = 90,
        UsesTotal = 91,
        UsesRemaining = 92,
        DateOfBirth = 98,
        Workmanship = 105,
        Spellcraft = 106,
        CurrentMana = 107,
        MaximumMana = 108,
        LoreRequirement = 109,
        RankRequirement = 110,
        PortalRestrictions = 111,
        Gender = 113,
        Attuned = 114,
        SkillLevelReq = 115,
        ManaCost = 117,
        Age = 125,
        XPForVPReduction = 129,
        Material = 131,
        WieldReqType = 158,
        WieldReqAttribute = 159,
        WieldReqValue = 160,
        SlayerSpecies = 166,
        CooldownSeconds = 167,
        NumberItemsSalvagedFrom = 170,
        NumberTimesTinkered = 171,
        DescriptionFormat = 172,
        PagesUsed = 174,
        PagesTotal = 175,
        ActivationReqSkillId = 176,
        GemSettingQty = 177,
        GemSettingType = 178,
        Imbued = 179,
        Heritage = 188,
        FishingSkill = 192,
        KeysHeld = 193,
        ElementalDmgBonus = 204,

        ArmorSetID = 265,

        ItemMaxLevel = 319, 
        CloakChanceType = 352, 
        WeaponMasteryCategory = 353,

        SummoningGemBuffedSkillReq = 367,
        SummoningGemLevelReq = 369,

        DamRating = 370,
        DamResistRating = 371,
        CritRating = 372,
        CritResistRating = 373,
        CritDamRating = 374,
        CritDamResistRating = 375,

        HealBoostRating = 376, 
        VitalityRating = 379,

        Type = 218103808,
        Icon = 218103809,
        Container = 218103810,
        Landblock = 218103811,
        ItemSlots = 218103812,
        PackSlots = 218103813,
        StackCount = 218103814,
        StackMax = 218103815,
        AssociatedSpell = 218103816,
        Slot = 218103817,
        Wielder = 218103818,
        WieldingSlot = 218103819,
        Monarch = 218103820,
        Coverage = 218103821,
        EquipableSlots = 218103822,
        EquipType = 218103823,
        IconOutline = 218103824,
        MissileType = 218103825,
        UsageMask = 218103826,
        HouseOwner = 218103827,
        HookMask = 218103828,
        HookType = 218103829,
        Model = 218103830,
        Flags = 218103831,
        CreateFlags1 = 218103832,
        CreateFlags2 = 218103833,
        Category = 218103834,
        Behavior = 218103835,
        MagicDef = 218103836,
        SpecialProps = 218103837,
        SpellCount = 218103838,
        WeapSpeed = 218103839,
        EquipSkill = 218103840,
        DamageType = 218103841,
        MaxDamage = 218103842,
        Unknown10 = 218103843,
        Unknown100000 = 218103844,
        Unknown800000 = 218103845,
        Unknown8000000 = 218103846,
        PhysicsDataFlags = 218103847,
        ActiveSpellCount = 218103848,
        IconOverlay = 218103849,
        IconUnderlay = 218103850
    }
    public enum PropertyFloat
    {
        ManaRateOfChange = 5,
        MeleeDefenseBonus = 29,
        ManaTransferEfficiency = 87,
        HealingKitRestoreBonus = 100,
        ManaStoneChanceDestruct = 137,
        ManaCBonus = 144,
        MissileDBonus = 149,
        MagicDBonus = 150,
        ElementalDamageVersusMonsters = 152,
        SlashProt = 167772160,
        PierceProt = 167772161,
        BludgeonProt = 167772162,
        AcidProt = 167772163,
        LightningProt = 167772164,
        FireProt = 167772165,
        ColdProt = 167772166,
        Heading = 167772167,
        ApproachDistance = 167772168,
        SalvageWorkmanship = 167772169,
        Scale = 167772170,
        Variance = 167772171,
        AttackBonus = 167772172,
        Range = 167772173,
        DamageBonus = 167772174,
    }

    public enum ObjectClass
    {
        Unknown = 0,
        MeleeWeapon = 1,
        Armor = 2,
        Clothing = 3,
        Jewelry = 4,
        Monster = 5,
        Food = 6,
        Money = 7,
        Misc = 8,
        MissileWeapon = 9,
        Container = 10,
        Gem = 11,
        SpellComponent = 12,
        Key = 13,
        Portal = 14,
        TradeNote = 15,
        ManaStone = 16,
        Plant = 17,
        BaseCooking = 18,
        BaseAlchemy = 19,
        BaseFletching = 20,
        CraftedCooking = 21,
        CraftedAlchemy = 22,
        CraftedFletching = 23,
        Player = 24,
        Vendor = 25,
        Door = 26,
        Corpse = 27,
        Lifestone = 28,
        HealingKit = 29,
        Lockpick = 30,
        WandStaffOrb = 31,
        Bundle = 32,
        Book = 33,
        Journal = 34,
        Sign = 35,
        Housing = 36,
        Npc = 37,
        Foci = 38,
        Salvage = 39,
        Ust = 40,
        Services = 41,
        Scroll = 42,
    }
#endif

    enum VTCSkillID
    {
        Axe = 1,
        Bow = 2,
        Crossbow = 3,
        Dagger = 4,
        Mace = 5,
        MeleeDefense = 6,
        MissileDefense = 7,
        Spear = 9,
        Staff = 10,
        Sword = 11,
        ThrownWeapons = 12,
        Unarmed = 13,
        ArcaneLore = 14,
        MagicDefense = 15,
        ManaConversion = 16,
        ItemTinkering = 18,
        AssessPerson = 19,
        Deception = 20,
        Healing = 21,
        Jump = 22,
        Lockpick = 23,
        Run = 24,
        AssessCreature = 27,
        WeaponTinkering = 28,
        ArmorTinkering = 29,
        MagicItemTinkering = 30,
        CreatureEnchantment = 31,
        ItemEnchantment = 32,
        LifeMagic = 33,
        WarMagic = 34,
        Leadership = 35,
        Loyalty = 36,
        Fletching = 37,
        Alchemy = 38,
        Cooking = 39,
        Salvaging = 40,
        TwoHandedCombat = 41,
        Gearcraft = 42,
        Void = 43,
        HeavyWeapons = 44,
        LightWeapons = 45,
        FinesseWeapons = 46,
        MissileWeapons = 47,
        Shield = 48,
        DualWield = 49,
        Recklessness = 50,
        SneakAttack = 51,
        DirtyFighting = 52,

        Summoning = 54
    }

    public enum eLootAction
    {
        NoLoot = 0,
        Keep = 1,
        Salvage = 2,
        Sell = 3,
        Read = 4,
        User1 = 5,
        User2 = 6,
        User3 = 7,
        User4 = 8,
        User5 = 9,
        KeepUpTo = 10,
    }

    // maybe some key-value collection will actually be better...
    internal class eLootActionTool
    {
        public static string FriendlyName(eLootAction e)
        {
            switch (e)
            {
                case eLootAction.Keep:
                    return "Keep";
                case eLootAction.Salvage:
                    return "Salvage";
                case eLootAction.Sell:
                    return "Sell";
                case eLootAction.Read:
                    return "Read";
                case eLootAction.KeepUpTo:
                    return "Keep #";
            }
            return string.Empty;
        }

        public static List<string> FriendlyNames()
        {
            List<string> r = new List<string>();

            foreach (eLootAction e in Enum.GetValues(typeof(eLootAction)))
            {
                string s = FriendlyName(e);
                if (!string.Empty.Equals(s))
                {
                    r.Add(s);
                }
            }

            return r;
        }

        public static eLootAction enumValue(string s)
        {
            foreach (eLootAction e in Enum.GetValues(typeof(eLootAction)))
            {
                string n = FriendlyName(e);
                if (s.Equals(n))
                {
                    return e;
                }
            }
            return eLootAction.NoLoot;
        }
    }

    internal interface iSettingsCollection
    {
        void Read(StreamReader inf, int fileversion);
        void Write(CountedStreamWriter inf);
    }

    internal class cUniqueID : IComparable<cUniqueID>
    {
        static long last = 0;
        public static cUniqueID New(int t, object n)
        {
            return new cUniqueID(++last, t, n);
        }

        public object node;
        public int type;
        long v;
        cUniqueID(long z, int t, object n)
        {
            node = n;
            type = t;
            v = z;
        }

        #region IComparable<cUniqueID> Members

        public int CompareTo(cUniqueID other)
        {
            return v.CompareTo(other);
        }

        #endregion
    }

    internal static class GameInfo
    {
        public static double HaxConvertDouble(string s)
        {
            string ss = s.Replace(',', '.');
            double res;
            if (!double.TryParse(ss, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out res))
                return 0d;
            return res;
        }

        public static bool IsIDProperty(StringValueKey vk)
        {
            return false;
            /*
            switch (vk) {
                case PropertyString.Name: return false;
                case PropertyString.SecondaryName: return false;
                default: return true;
            }
            */
        }

        public static bool IsIDProperty(IntValueKey vk)
        {
            return false;
            /*
            switch (vk) {
                case PropertyInt.CreateFlags1: return false;
                case PropertyInt.Type: return false;
                case PropertyInt.Icon: return false;
                case PropertyInt.Category: return false;
                case PropertyInt.Behavior: return false;
                case PropertyInt.CreateFlags2: return false;
                case PropertyInt.IconUnderlay: return false;
                case PropertyInt.ItemSlots: return false;
                case PropertyInt.PackSlots: return false;
                case PropertyInt.MissileType: return false;
                case PropertyInt.Value: return false;
                //case PropertyInt.Unknown10: return false;
                case PropertyInt.UsageMask: return false;
                case PropertyInt.IconOutline: return false;
                case PropertyInt.EquipType: return false;
                case PropertyInt.UsesRemaining: return false;
                case PropertyInt.UsesTotal: return false;
                case PropertyInt.StackCount: return false;
                case PropertyInt.StackMax: return false;
                case PropertyInt.Container: return false;
                case PropertyInt.Slot: return false;
                case PropertyInt.EquipableSlots: return false;
                case PropertyInt.EquippedSlots: return false;
                case PropertyInt.Coverage: return false;
                //case PropertyInt.Unknown100000: return false;
                //case PropertyInt.Unknown800000: return false;
                case PropertyInt.Unknown8000000: return false;
                //case PropertyInt.Burden: return false;
                //case PropertyInt.OwnedBy: return false;
                case PropertyInt.Monarch: return false;
                case PropertyInt.HookMask: return false;
                case PropertyInt.IconOverlay: return false;
                case PropertyInt.Material: return false;
                default: return true;
            }
            */
        }

        public static bool IsIDProperty(DoubleValueKey vk)
        {
            return false;
            /*
            switch (vk) {
                case PropertyFloat.ApproachDistance: return false;
                case PropertyFloat.SalvageWorkmanship: return false;
                default: return true;
            }
            */
        }

        public static SortedDictionary<string, int> getSetInfo()
        {
            int i = 32;
            SortedDictionary<string, int> r = new SortedDictionary<string, int>();
            r.Add("Protective Clothing", i--);
            r.Add("Gladiatorial Clothing", i--);
            r.Add("Dedication", i--);
            r.Add("Lightning Proof", i--);
            r.Add("Cold Proof", i--);
            r.Add("Acid Proof", i--);
            r.Add("Flame Proof", i--);
            r.Add("Interlocking", i--);
            r.Add("Reinforced", i--);
            r.Add("Hardenend", i--);
            r.Add("Swift", i--);
            r.Add("Wise", i--);
            r.Add("Dexterous", i--);
            r.Add("Hearty", i--);
            r.Add("Crafter's", i--);
            r.Add("Tinker's", i--);
            r.Add("Defender's", i--);
            r.Add("Archer's", i--);
            r.Add("Adept's", i--);
            r.Add("Soldier's", i--);
            r.Add("Leggings of Perfect Light", i--);
            r.Add("Coat of the Perfect Light", i--);
            r.Add("Arm, Mind, Heart", i--);
            r.Add("Empyrean Rings", i--);
            r.Add("Shou-jen", i--);
            r.Add("Relic Alduressa", i--);
            r.Add("Ancient Relic", i--);
            r.Add("Noble Relic", i--);

            return r;
        }

        public static SortedDictionary<string, int> getSkillInfo()
        {
            SortedDictionary<string, int> r = new SortedDictionary<string, int>();
            r.Add("Axe (DEPRECEATED)", 1);
            r.Add("Bow (DEPRECEATED)", 2);
            r.Add("Crossbow (DEPRECEATED)", 3);
            r.Add("Dagger (DEPRECEATED)", 4);
            r.Add("Mace (DEPRECEATED)", 5);
            r.Add("MeleeD", 6);
            r.Add("MissileD", 7);
            r.Add("Spear (DEPRECEATED)", 9);
            r.Add("Staff (DEPRECEATED)", 10);
            r.Add("Sword (DEPRECEATED)", 11);
            r.Add("Thrown Weapons (DEPRECEATED)", 12);
            r.Add("Unarmed Combat (DEPRECEATED)", 13);
            r.Add("MagicD", 15);
            r.Add("ManaCon", 16);
            r.Add("Creature", 31);
            r.Add("Item", 32);
            r.Add("Life", 33);
            r.Add("War", 34);
            r.Add("Two-Handed Combat", 41);
            r.Add("Void", 43);
            r.Add("Heavy Weapons", 44);
            r.Add("Light Weapons", 45);
            r.Add("Finesse Weapons", 46);
            r.Add("Missile Weapons", 47);
            r.Add("Shield", 48);
            r.Add("Dual Wield", 49);
            r.Add("Recklessness", 50);
            r.Add("Sneak Attack", 51);
            r.Add("Dirty Fighting", 52);
            r.Add("Summoning", 54);

            return r;
        }

        public static SortedDictionary<string, int> getMasteryInfo()
        {
            SortedDictionary<string, int> r = new SortedDictionary<string, int>();

            r.Add("Unarmed Weapon", 1);
            r.Add("Sword", 2);
            r.Add("Axe", 3);
            r.Add("Mace", 4);
            r.Add("Spear", 5);
            r.Add("Dagger", 6);
            r.Add("Staff", 7);
            r.Add("Bow", 8);
            r.Add("Crossbow", 9);
            r.Add("Thrown", 10);
            r.Add("Two Handed Combat", 11);

            return r;
        }

        private static SortedDictionary<string, int> matIds;
        private static SortedDictionary<int, string> matNames;
        static void GenerateMaterialInfo()
        {
            if (matIds == null)
            {
                matIds = new SortedDictionary<string, int>();
                matIds.Add("Agate", 10);
                matIds.Add("Alabaster", 66);
                matIds.Add("Amber", 11);
                matIds.Add("Amethyst", 12);
                matIds.Add("Aquamarine", 13);
                matIds.Add("Armoredillo Hide", 53);
                matIds.Add("Azurite", 14);
                matIds.Add("Black Garnet", 15);
                matIds.Add("Black Opal", 16);
                matIds.Add("Bloodstone", 17);
                matIds.Add("Brass", 57);
                matIds.Add("Bronze", 58);
                matIds.Add("Carnelian", 18);
                matIds.Add("Ceramic", 1);
                matIds.Add("Citrine", 19);
                matIds.Add("Copper", 59);
                matIds.Add("Diamond", 20);
                matIds.Add("Ebony", 73);
                matIds.Add("Emerald", 21);
                matIds.Add("Fire Opal", 22);
                matIds.Add("Gold", 60);
                matIds.Add("Granite", 67);
                matIds.Add("Green Garnet", 23);
                matIds.Add("Green Jade", 24);
                matIds.Add("Gromnie Hide", 54);
                matIds.Add("Hematite", 25);
                matIds.Add("Imperial Topaz", 26);
                matIds.Add("Iron", 61);
                matIds.Add("Ivory", 51);
                matIds.Add("Jet", 27);
                matIds.Add("Lapis Lazuli", 28);
                matIds.Add("Lavender Jade", 29);
                matIds.Add("Leather", 52);
                matIds.Add("Linen", 4);
                matIds.Add("Mahogany", 74);
                matIds.Add("Malachite", 30);
                matIds.Add("Marble", 68);
                matIds.Add("Moonstone", 31);
                matIds.Add("Oak", 75);
                matIds.Add("Obsidian", 69);
                matIds.Add("Onyx", 32);
                matIds.Add("Opal", 33);
                matIds.Add("Peridot", 34);
                matIds.Add("Pine", 76);
                matIds.Add("Porcelain", 2);
                matIds.Add("Pyreal", 62);
                matIds.Add("Red Garnet", 35);
                matIds.Add("Red Jade", 36);
                matIds.Add("Reed Shark Hide", 55);
                matIds.Add("Rose Quartz", 37);
                matIds.Add("Ruby", 38);
                matIds.Add("Sandstone", 70);
                matIds.Add("Sapphire", 39);
                matIds.Add("Satin", 5);
                matIds.Add("Serpentine", 71);
                matIds.Add("Silk", 6);
                matIds.Add("Silver", 63);
                matIds.Add("Smokey Quartz", 40);
                matIds.Add("Steel", 64);
                matIds.Add("Sunstone", 41);
                matIds.Add("Teak", 77);
                matIds.Add("Tiger Eye", 42);
                matIds.Add("Tourmaline", 43);
                matIds.Add("Turquoise", 44);
                matIds.Add("Velvet", 7);
                matIds.Add("White Jade", 45);
                matIds.Add("White Quartz", 46);
                matIds.Add("White Sapphire", 47);
                matIds.Add("Wool", 8);
                matIds.Add("Yellow Garnet", 48);
                matIds.Add("Yellow Topaz", 49);
                matIds.Add("Zircon", 50);

                matNames = new SortedDictionary<int, string>();
                foreach (KeyValuePair<string, int> kp in matIds)
                {
                    matNames[kp.Value] = kp.Key;
                }
            }
        }
        public static SortedDictionary<string, int> getMaterialInfo()
        {
            GenerateMaterialInfo();
            return matIds;
        }

        public static SortedDictionary<int, string> getMaterialNamesByID()
        {
            GenerateMaterialInfo();
            return matNames;
        }

        public static string getMaterialName(int materialId)
        {
            GenerateMaterialInfo();
            if (matNames.ContainsKey(materialId))
                return matNames[materialId];
            else
                return string.Empty;
        }

        public static int GetMaterialID(string matname)
        {
            GenerateMaterialInfo();
            if (matIds.ContainsKey(matname))
                return matIds[matname];
            else
                return 0;
        }

        public static SortedDictionary<string, int[]> getMaterialGroups()
        {
            SortedDictionary<string, int[]> r = new SortedDictionary<string, int[]>();
            // Armor Tinkering: Alabaster, Armoredillo Hide, Bronze, Ceramic, Marble, Peridot, Reedshark Hide, Steel, Wool, Yellow Topaz, Zircon
            r.Add("Armor Tinkering", new int[] { 66, 53, 58, 1, 68, 34, 55, 64, 8, 49, 50 });
            // Item Tinkering: Copper, Ebony, Gold, Linen, Pine, Porcelain, Moonstone, Silk, Silver, Teak
            r.Add("Item Tinkering", new int[] { 59, 73, 60, 4, 76, 2, 31, 6, 63, 77 });
            // Magic Item Tinkering: Agate, Azurite, Black Opal, Bloodstone, Carnelian, Citrine, Fire Opal, Hematite, Lavender Jade, Malachite, Opal, Red Jade, Rose Quartz, Sunstone
            r.Add("Magic Item Tinkering", new int[] { 10, 14, 16, 17, 18, 19, 22, 25, 29, 30, 33, 36, 37, 41 });
            // Weapon Tinkering: Aquamarine, Black Garnet, Brass, Emerald, Granite, Iron, Imperial Topaz, Jet, Mahogany, Oak, Red Garnet, Velvet, White Sapphire
            r.Add("Weapon Tinkering", new int[] { 13, 15, 57, 21, 67, 61, 26, 27, 74, 75, 35, 7, 47 });
            // Basic Tinkering: Ivory, Leather
            r.Add("Basic Tinkering", new int[] { 51, 52 });
            // Gearcrafting: Amber, Diamond, Gromnie Hide, Pyreal, Ruby, Sapphire
            r.Add("Gearcrafting", new int[] { 11, 20, 54, 62, 38, 39 });
            // Armor Imbues: Peridot, Yellow Topaz, Zircon
            r.Add("Armor Imbues", new int[] { 34, 49, 50 });
            // Protection Tinks: Alabaster, Armoredillo Hide, Bronze, Ceramic, Marble, Reedshark Hide, Steel, Wool
            r.Add("Protection Tinks", new int[] { 66, 53, 58, 1, 68, 55, 64, 8 });
            // Weapon Imbues: Aquamarine, Black Garnet, Emerald, Imperial Topaz, Jet, Red Garnet, White Sapphire, Sunstone, Black Opal
            r.Add("Weapon Imbues", new int[] { 13, 15, 21, 26, 27, 35, 47, 41, 16 });
            // Brass/Iron/Granite/Hog
            r.Add("Brass/Granite/Iron/Mahog", new int[] { 57, 67, 61, 74 });
            // RG/BG/Jet
            r.Add("RG/BG/Jet", new int[] { 35, 15, 27 });
            return r;
        }

    }

}