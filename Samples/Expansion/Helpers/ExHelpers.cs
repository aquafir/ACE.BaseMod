using ACE.Server.Network.GameEvent.Events;
using ACE.Server.WorldObjects.Entity;
using System.Text;
using static ACE.Server.WorldObjects.Player;
using Expansion.Creatures;
using ACE.Server.Physics;
using ACE.Database;
using ACE.Server.Network.GameMessages.Messages;

namespace Expansion.Helpers;

public static class CreatureExHelpers
{
    public static CreatureEx Create(this Creatures.CreatureExType type, Biota biota) => type switch
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
    public static CreatureEx Create(this Creatures.CreatureExType type, Weenie weenie, ObjectGuid guid) => type switch
    {
        Creatures.CreatureExType.Accurate => new Accurate(weenie, guid),
        //Creatures.CreatureType.Avenger => new Avenger(weenie, guid),
        //Creatures.CreatureType.Bard => new Bard(weenie, guid),
        Creatures.CreatureExType.Boss => new Boss(weenie, guid),
        Creatures.CreatureExType.Berserker => new Berserker(weenie, guid),
        Creatures.CreatureExType.Comboer => new Comboer(weenie, guid),
        Creatures.CreatureExType.Drainer => new Drainer(weenie, guid),
        Creatures.CreatureExType.Duelist => new Duelist(weenie, guid),
        Creatures.CreatureExType.Evader => new Evader(weenie, guid),
        Creatures.CreatureExType.Exploding => new Exploder(weenie, guid),
        Creatures.CreatureExType.Healer => new Creatures.Healer(weenie, guid),
        Creatures.CreatureExType.Merger => new Merger(weenie, guid),
        //Creatures.CreatureType.Necromancer => new Necromancer(weenie, guid),
        //Creatures.CreatureType.Poisoner => new Poisoner(weenie, guid),
        Creatures.CreatureExType.Puppeteer => new Puppeteer(weenie, guid),
        //Creatures.CreatureType.Reaper => new Reaper(weenie, guid),
        Creatures.CreatureExType.Rogue => new Rogue(weenie, guid),
        //Creatures.CreatureType.Runner => new Runner(weenie, guid),
        Creatures.CreatureExType.Shielded => new Shielded(weenie, guid),
        Creatures.CreatureExType.SpellBreaker => new SpellBreaker(weenie, guid),
        Creatures.CreatureExType.SpellThief => new SpellThief(weenie, guid),
        //Creatures.CreatureType.Splitter => new Splitter(weenie, guid),
        //Creatures.CreatureType.Stomper => new Stomper(weenie, guid),
        Creatures.CreatureExType.Stunner => new Stunner(weenie, guid),
        //Creatures.CreatureType.Suppresser => new Suppresser(weenie, guid),
        Creatures.CreatureExType.Tank => new Tank(weenie, guid),
        Creatures.CreatureExType.Vampire => new Vampire(weenie, guid),
        Creatures.CreatureExType.Warder => new Warder(weenie, guid),
        _ => new Stunner(weenie, guid),      //throw new NotImplementedException(),
    };
}

