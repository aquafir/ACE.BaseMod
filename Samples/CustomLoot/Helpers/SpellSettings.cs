﻿using ACE.DatLoader;
using ACE.DatLoader.Entity;
using ACE.Entity;
using ACE.Server.Command;
using ACE.Server.Network;
using System;
using System.Text;
using static ACE.Server.WorldObjects.Player;

namespace CustomLoot.Helpers;

public class SpellSettings
{
    public string PortalDatPath { get; set; } = @"C:\ACE\Dats\client_portal.dat";

    public bool ReplaceBeforeCast { get; set; } = false;    //Switching between patch use for a full spell replacement or just the created spell

    public SpellGroupType GroupType { get; set; } = SpellGroupType.R;
    public SpellGroupType LastGenerated { get; set; } = SpellGroupType.None;

    public bool OnlyPlayerSpells { get; set; } = true;      //If true only spells in the PlayerTable will be made into groups when creating
    public bool DumpSpellGroups { get; set; } = false;      //Outputs a description of some of the SpellBase data for groups when creating

    public bool DifferentInDungeon { get; set; } = true;    //Uses the landblock of a dungeon to shift a spell to a comparable one in its group
    public bool RandomizeSpells { get; set; } = true;       //If a spell isn't changed by being in a dungeon with the above enabled, this will randomize it

    public bool FistMagic { get; set; } = true;             //UA casts from the pool
    public uint[] FistPool { get; set; } =                  //Rings
        { 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789 };
    public uint FistBuckets { get; set; } = 6;              //"Buckets" per power/acc slider, default based on increment of keyboard change
    [JsonIgnore]
    public uint TotalBuckets => FistBuckets * 3;            //One for each heights

    public bool SpellsSplash { get; set; } = true;
    public int SplashCount { get; set; } = 3;
    public float SplashRange { get; set; } = 5.0f;
    public TimeSpan SplashCooldown { get; set; } = TimeSpan.FromSeconds(5);

    public bool SplitSpells { get; set; } = true;
    public int SplitCount { get; set; } = 5;
    public float SplitRange { get; set; } = 35.0f;
    public TimeSpan SplitCooldown { get; set; } = TimeSpan.FromSeconds(2);

    public bool MetaEnabled { get; set; } = true;
    public double MetaScale { get; set; } = 1;
}

public enum SpellGroupType
{
    None,
    C,     //Lazy aliases
    R,

    /// <summary>
    /// Comparable spells are more tight matches, such as Fire 1-8
    /// </summary>
    Comparable = C,
    /// <summary>
    /// Related spells have similar targets and schools
    /// </summary>
    Related = R,
}
