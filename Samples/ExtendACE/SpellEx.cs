namespace Tinkering;

public class SpellEx : Spell
{
    public SpellEx(uint spellID, bool loadDB = true) : base(spellID, loadDB)
    {
    }

    public SpellEx(int spellID, bool loadDB = true) : base(spellID, loadDB)
    {
    }

    public SpellEx(SpellId spell, bool loadDB = true) : base(spell, loadDB)
    {
    }

}
