namespace Bank;

public class Settings
{
    public bool VendorsUseBank { get; set; } = true;
    public bool DirectDeposit { get; set; } = true;

    public int MaxCoinsDropped { get; set; } = 100000;

    public bool CoinExplosion { get; set; } = true;
    public int MaxPiles { get; set; } = 10;
    public uint PileWcid { get; set; } = Player.coinStackWcid;
    public int PileValue { get; set; } = 25000;
    public float ExplosionVelocity { get; set; } = 6;

    //WCID - PropInt64
    public List<BankItem> Items { get; set; } = new()
    {
        new ("MMD", 20630, 40000),
        new ("Infused Amber Shard",52968, 40001),
        new ("Small Olthoi Venom Sac", 36376, 40002),
        new ("A'nekshay Token", 44240, 40003),
        new ("Ornate Gear Marker", 43142, 40004),
        new ("Colosseum Coin",36518, 40005),
        new ("Ancient Mhoire Coin", 35383, 40006),
        new ("Promissory Note", 43901, 40007),
    };
    //Reduces amount to a cap
    public bool ExcessSetToMax { get; set; } = true;


    public int LuminanceProperty { get; set; } = 39998;
    public int CashProperty { get; set; } = 39999;

    public List<CurrencyItem> Currencies { get; set; } = new()
    {
        new ("Pyreal", 273, 1),
        new ("I", 2621,  100),
        new ("V", 2622,  500),
        new ("X", 2623,  1000),
        new ("L", 2624,  5000),
        new ("C", 2625,  10000),
        new ("D", 2626,  50000),
        new ("M", 2627,  100000),
        new ("CL", 7374,  15000),
        new ("CC", 7375,  20000),
        new ("CCL", 7376,  25000),
        new ("DCCL", 7377,  75000),
        new ("MD", 20628,  150000),
        new ("MM", 20629,  200000),
        new ("MMD", 20630,  250000),
        new ("CashSink", 40652,  50*250000),
    };

}

public record BankItem(string Name, uint Id, int Prop);
public record CurrencyItem(string Name, uint Id, int Value);


