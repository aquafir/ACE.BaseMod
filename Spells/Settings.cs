namespace Spells
{
    public class Settings
    {
        public string PortalDatPath { get; set; } = @"C:\ACE\Dats\client_portal.dat";

        public bool ReplaceBeforeCast { get; set; } = true;    //Switching between patch use for a full spell replacement or just the created spell

        public bool UseComparable { get; set; } = true;         //If false less precise "related" spells are used

        public bool OnlyPlayerSpells { get; set; } = true;      //If true only spells in the PlayerTable will be made into groups when creating
        public bool DumpSpellGroups { get; set; } = false;      //Outputs a description of some of the SpellBase data for groups when creating

        public bool DifferentInDungeon { get; set; } = true;    //Uses the landblock of a dungeon to shift a spell to a comparable one in its group
        public bool RandomizeSpells { get; set; } = true;       //If a spell isn't changed by being in a dungeon with the above enabled, this will randomize it

        public bool FistMagic { get; set; } = true;             //UA casts from the pool
        public uint[] FistPool { get; set; } =              
            { 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789 };
    }
}