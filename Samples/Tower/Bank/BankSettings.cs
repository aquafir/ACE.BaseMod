namespace Tower;

public class BankSettings
{
    //public bool VendorsUseBank { get; set; } = true;
    //Doesn't print blank banked items
    public bool SkipMissingBankedItems { get; set; } = true;

    //Reduces amount to a cap
    public bool ExcessSetToMax { get; set; } = true;

    //WCID - PropInt64
    public List<BankItem> Items { get; set; } = new()
    {
        new ("MMD", 20630, 40000),
    };
}

public record BankItem(string Name, uint Id, int Prop);
