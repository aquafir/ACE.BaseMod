﻿namespace Tinkering;

public class Settings
{
    //Maximum connections allowed in non-exempt zones
    public int MaxNonExempt { get; set; } = 1;

    //Number of seconds between checking
    public double Interval { get; set; } = 10;

    //IP addresses with the number of allowed connections that are exempt
    public Dictionary<string, int> ExemptIPAddresses { get; set; } = new();

    //Landblocks that are exempt
    public HashSet<ushort> ExemptLandblocks { get; set; } = new()
        {
            //MP
            0x016C,
            //Appartments
            0x7200, 0x7300, 0x7400, 0x7500, 0x7600, 0x7700, 0x7800, 0x7900, 0x7A00, 0x7B00, 0x7C00, 0x7D00, 0x7E00, 0x7F00, 0x8000, 0x8100, 0x8200, 0x8300, 0x8400, 0x8500, 0x8600, 0x8700, 0x8800, 0x8900, 0x8A00, 0x8B00, 0x8C00, 0x8D00, 0x8E00, 0x8F00, 0x9000, 0x9100, 0x9200, 0x9300, 0x9400, 0x9500, 0x9600, 0x9700, 0x9800, 0x9900, 0x5360, 0x5361, 0x5362, 0x5363, 0x5364, 0x5365, 0x5366, 0x5367, 0x5368, 0x5369
        };
}
