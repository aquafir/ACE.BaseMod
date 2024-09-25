
namespace CustomSpells;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(CustomSpells), new PatchClass(this));
}

public class PatchClas(BasicMod mod, string settingsName = "Settings.json") : BasicPatch<Settings>(mod, settingsName)
{
    public override async Task OnStartSuccess()
    {
        //Stuff you do as soon as settings are loaded
    }
    public override async Task OnWorldOpen()
    {
        //Stuff you do after waiting for the World to be open
    }
    public override void Stop()
    {
        base.Stop();
        
        //Stuff you do to settings change
        //Full mod removal done in Dispose will also call this
    }
}