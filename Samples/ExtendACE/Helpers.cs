using ACE.Server.Network.GameEvent.Events;
using ACE.Server.WorldObjects.Entity;
using System.Text;
using static ACE.Server.WorldObjects.Player;
using ExtendACE.Creatures;
using ACE.Server.Physics;
using ACE.Database;
using ACE.Server.Network.GameMessages.Messages;

namespace ExtendACE;

public static class CreatureExHelpers
{
    public static CreatureEx Create(this Creatures.CreatureType type, Biota biota) => type switch
    {
        //Creatures.CreatureType.Accurate => new Accurate(biota),
        //Creatures.CreatureType.Avenger => new Avenger(biota),
        //Creatures.CreatureType.Bard => new Bard(biota),
        //Creatures.CreatureType.Boss => new Boss(biota),
        //Creatures.CreatureType.Berserker => new Berserker(biota),
        //Creatures.CreatureType.Comboer => new Comboer(biota),
        //Creatures.CreatureType.Drainer => new Drainer(biota),
        //Creatures.CreatureType.Duelist => new Duelist(biota),
        //Creatures.CreatureType.Evader => new Evader(biota),
        //Creatures.CreatureType.Exploding => new Exploder(biota),
        //Creatures.CreatureType.Healer => new Healer(biota),
        //Creatures.CreatureType.Launcher => new Launcher(biota),
        //Creatures.CreatureType.Merging => new Merger(biota),
        //Creatures.CreatureType.Necromancer => new Necromancer(biota),
        //Creatures.CreatureType.Poisoner => new Poisoner(biota),
        //Creatures.CreatureType.Puppeteer => new Puppeteer(biota),
        //Creatures.CreatureType.Reaper => new Reaper(biota),
        //Creatures.CreatureType.Rogue => new Rogue(biota),
        //Creatures.CreatureType.Runner => new Runner(biota),
        //Creatures.CreatureType.Shielded => new Shielded(biota),
        //Creatures.CreatureType.SpellBreaker => new SpellBreaker(biota),
        //Creatures.CreatureType.SpellThief => new SpellThief(biota),
        //Creatures.CreatureType.Splitter => new Splitter(biota),
        //Creatures.CreatureType.Stomper => new Stomper(biota),
        //Creatures.CreatureType.Stunner => new Stunner(biota),
        //Creatures.CreatureType.Suppresser => new Suppresser(biota),
        //Creatures.CreatureType.Tank => new Tank(biota),
        //Creatures.CreatureType.Vampire => new Vampire(biota),
        //Creatures.CreatureType.Warder => new Warder(biota),
        _ => new CreatureEx(biota),             // throw new NotImplementedException(),
    };
    public static CreatureEx Create(this Creatures.CreatureType type, Weenie weenie, ObjectGuid guid) => type switch
    {
        Creatures.CreatureType.Accurate => new Accurate(weenie, guid),
        //Creatures.CreatureType.Avenger => new Avenger(weenie, guid),
        //Creatures.CreatureType.Bard => new Bard(weenie, guid),
        Creatures.CreatureType.Boss => new Boss(weenie, guid),
        Creatures.CreatureType.Berserker => new Berserker(weenie, guid),
        Creatures.CreatureType.Comboer => new Comboer(weenie, guid),
        Creatures.CreatureType.Drainer => new Drainer(weenie, guid),
        Creatures.CreatureType.Duelist => new Duelist(weenie, guid),
        Creatures.CreatureType.Evader => new Evader(weenie, guid),
        Creatures.CreatureType.Exploding => new Exploder(weenie, guid),
        Creatures.CreatureType.Healer => new Creatures.Healer(weenie, guid),
        Creatures.CreatureType.Merger => new Merger(weenie, guid),
        //Creatures.CreatureType.Necromancer => new Necromancer(weenie, guid),
        //Creatures.CreatureType.Poisoner => new Poisoner(weenie, guid),
        Creatures.CreatureType.Puppeteer => new Puppeteer(weenie, guid),
        //Creatures.CreatureType.Reaper => new Reaper(weenie, guid),
        Creatures.CreatureType.Rogue => new Rogue(weenie, guid),
        //Creatures.CreatureType.Runner => new Runner(weenie, guid),
        Creatures.CreatureType.Shielded => new Shielded(weenie, guid),
        Creatures.CreatureType.SpellBreaker => new SpellBreaker(weenie, guid),
        Creatures.CreatureType.SpellThief => new SpellThief(weenie, guid),
        //Creatures.CreatureType.Splitter => new Splitter(weenie, guid),
        //Creatures.CreatureType.Stomper => new Stomper(weenie, guid),
        Creatures.CreatureType.Stunner => new Stunner(weenie, guid),
        //Creatures.CreatureType.Suppresser => new Suppresser(weenie, guid),
        Creatures.CreatureType.Tank => new Tank(weenie, guid),
        Creatures.CreatureType.Vampire => new Vampire(weenie, guid),
        Creatures.CreatureType.Warder => new Warder(weenie, guid),
        _ => new Stunner(weenie, guid),      //throw new NotImplementedException(),
    };
}

