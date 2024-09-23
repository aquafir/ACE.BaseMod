namespace Bank;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(Bank), new PatchClass(this));
}