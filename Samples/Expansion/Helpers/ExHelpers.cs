﻿namespace Expansion.Helpers;

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
#if REALM
        CreatureExType.Accurate => new Accurate(weenie, guid, null),
        //Creatures.CreatureType.Avenger => new Avenger(weenie, guid, null),
        CreatureExType.Banisher => new Banisher(weenie, guid, null),
        //Creatures.CreatureType.Bard => new Bard(weenie, guid, null),
        CreatureExType.Berserker => new Berserker(weenie, guid, null),
        CreatureExType.Boss => new Boss(weenie, guid, null),
        CreatureExType.Comboer => new Comboer(weenie, guid, null),
        CreatureExType.Drainer => new Drainer(weenie, guid, null),
        CreatureExType.Duelist => new Duelist(weenie, guid, null),
        CreatureExType.Evader => new Evader(weenie, guid, null),
        CreatureExType.Exploder => new Exploder(weenie, guid, null),
        CreatureExType.Healer => new Creatures.Healer(weenie, guid, null),
        CreatureExType.Merger => new Merger(weenie, guid, null),
        //Creatures.CreatureType.Necromancer => new Necromancer(weenie, guid, null),
        //Creatures.CreatureType.Poisoner => new Poisoner(weenie, guid, null),
        CreatureExType.Puppeteer => new Puppeteer(weenie, guid, null),
        //Creatures.CreatureType.Reaper => new Reaper(weenie, guid, null),
        CreatureExType.Rogue => new Rogue(weenie, guid, null),
        //Creatures.CreatureType.Runner => new Runner(weenie, guid, null),
        CreatureExType.Shielded => new Shielded(weenie, guid, null),
        CreatureExType.SpellBreaker => new SpellBreaker(weenie, guid, null),
        CreatureExType.SpellThief => new SpellThief(weenie, guid, null),
        //Creatures.CreatureType.Splitter => new Splitter(weenie, guid, null),
        //Creatures.CreatureType.Stomper => new Stomper(weenie, guid, null),
        CreatureExType.Stunner => new Stunner(weenie, guid, null),
        //Creatures.CreatureType.Suppresser => new Suppresser(weenie, guid, null),
        CreatureExType.Tank => new Tank(weenie, guid, null),
        CreatureExType.Vampire => new Vampire(weenie, guid, null),
        CreatureExType.Warder => new Warder(weenie, guid, null),
        _ => new Stunner(weenie, guid, null),      //throw new NotImplementedException(),
#else
        CreatureExType.Accurate => new Accurate(weenie, guid),
        //Creatures.CreatureType.Avenger => new Avenger(weenie, guid),
        CreatureExType.Banisher => new Banisher(weenie, guid),
        //Creatures.CreatureType.Bard => new Bard(weenie, guid),
        CreatureExType.Berserker => new Berserker(weenie, guid),
        CreatureExType.Boss => new Boss(weenie, guid),
        CreatureExType.Comboer => new Comboer(weenie, guid),
        CreatureExType.Drainer => new Drainer(weenie, guid),
        CreatureExType.Duelist => new Duelist(weenie, guid),
        CreatureExType.Evader => new Evader(weenie, guid),
        CreatureExType.Exploder => new Exploder(weenie, guid),
        CreatureExType.Healer => new Creatures.Healer(weenie, guid),
        CreatureExType.Merger => new Merger(weenie, guid),
        //Creatures.CreatureType.Necromancer => new Necromancer(weenie, guid),
        //Creatures.CreatureType.Poisoner => new Poisoner(weenie, guid),
        CreatureExType.Puppeteer => new Puppeteer(weenie, guid),
        //Creatures.CreatureType.Reaper => new Reaper(weenie, guid),
        CreatureExType.Rogue => new Rogue(weenie, guid),
        //Creatures.CreatureType.Runner => new Runner(weenie, guid),
        CreatureExType.Shielded => new Shielded(weenie, guid),
        CreatureExType.SpellBreaker => new SpellBreaker(weenie, guid),
        CreatureExType.SpellThief => new SpellThief(weenie, guid),
        //Creatures.CreatureType.Splitter => new Splitter(weenie, guid),
        //Creatures.CreatureType.Stomper => new Stomper(weenie, guid),
        CreatureExType.Stunner => new Stunner(weenie, guid),
        //Creatures.CreatureType.Suppresser => new Suppresser(weenie, guid),
        CreatureExType.Tank => new Tank(weenie, guid),
        CreatureExType.Vampire => new Vampire(weenie, guid),
        CreatureExType.Warder => new Warder(weenie, guid),
        _ => new Stunner(weenie, guid),      //throw new NotImplementedException(),
#endif
    };
}

