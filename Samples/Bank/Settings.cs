﻿namespace Bank;

public class Settings
{
    //WCID - PropInt64
    public List<BankItem> Items { get; set; } = new()
    {
        new ("MMD", 20630, 40000),
        new ("Concentrated Fire Oil", 9346, 40001),
        new ("Fire Oil", 4750, 40002),
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
