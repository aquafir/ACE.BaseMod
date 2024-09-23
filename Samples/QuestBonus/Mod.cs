namespace QuestBonus;

public class Mod : BasicMod
{
    public Mod() : base() => Setup(nameof(QuestBonus), new PatchClass(this));
}