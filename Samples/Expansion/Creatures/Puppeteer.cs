namespace Expansion.Creatures;

[HarmonyPatchCategory(nameof(CreatureExType.Puppeteer))]
public class Puppeteer : CreatureEx
{
    public Puppeteer(Biota biota) : base(biota) { }
#if REALM
    public Puppeteer(Weenie weenie, ObjectGuid guid, AppliedRuleset ruleset) : base(weenie, guid, ruleset)
#else
    public Puppeteer(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
#endif
    { }

    private PropertiesGenerator generator;

    //Mutate from the original weenie
    protected override void Initialize()
    {
        base.Initialize();

        //Skip weenies with generators?
        if (GeneratorProfiles is null || GeneratorProfiles.Count > 0)
            return;

        Name = "Conniving " + Name;

        generator = new()
        {
            WhenCreate = RegenerationType.Destruction,
            WhereCreate = RegenLocationType.Scatter,
            Probability = -1,
            WeenieClassId = WeenieClassId,
            Shade = .5f,
            InitCreate = 1,
            MaxCreate = 3,
            Delay = 15,
            AnglesW = 1,
            StackSize = -5,
        };

        GeneratorProfiles = new();
        GeneratorProfiles.Add(new GeneratorProfile(this, generator, 0));
    }


    //Custom behavior
    double last = Timers.PortalYearTicks;
    private List<Creature> children = new();
    public override void Heartbeat(double currentUnixTime)
    {
        base.Heartbeat(currentUnixTime);

        if (generator is null || generator.MaxCreate <= children.Count) return;

        var lapsed = currentUnixTime - last;
        if (lapsed < generator.Delay) return;
        lapsed = currentUnixTime;

        //for(var i = 0; i < generator.MaxCreate; i++)
        //{
        //    CreatureEx creature = new CreatureEx(this.Weenie, GuidManager.NewDynamicGuid());
        //    creature.Location = this.Location.Translate(5, (float)(Math.PI * 2.0 / (i / generator.MaxCreate)));
        //    creature.Location.FindZ();
        //    creature.EnterWorld();
        //}


        //var setPos = new ACE.Server.Physics.Common.SetPosition(newLoc.PhysPosition(), Physics.Common.SetPositionFlags.Teleport | Physics.Common.SetPositionFlags.Slide);
        //var result = obj.PhysicsObj.SetPosition(setPos);

        //if (result != Physics.Common.SetPositionError.OK)
        //{
        //    session.Network.EnqueueSend(new GameMessageSystemChat($"Failed to move {obj.Name} ({obj.Guid}) to current location: {result}", ChatMessageType.Broadcast));
        //    return;
        //}

        var spawns = GeneratorProfiles.FirstOrDefault()?.Spawn();
        foreach (var spawn in spawns)
        {
            if (spawn is Creature creature)
            {
                creature.Attackable = false;
                children.Add(creature);
            }
        }
    }

    //Kill puppets on death
    public override DeathMessage OnDeath(DamageHistoryInfo lastDamager, DamageType damageType, bool criticalHit = false)
    {
        foreach (var c in children)
        {
            c.OnDeath();
            c.Die();
        }
        children.Clear();

        return base.OnDeath(lastDamager, damageType, criticalHit);
    }

}