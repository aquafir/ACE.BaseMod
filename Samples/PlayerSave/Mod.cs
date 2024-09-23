namespace PlayerSave;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(PlayerSave), new PatchClass(this));
}