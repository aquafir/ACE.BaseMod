﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Hardcore;
public class HardcoreSettings
{
    public double SecondsBetweenDeathAllowed { get; set; } = TimeSpan.FromDays(365).TotalSeconds;
    public int HardcoreStartingLives { get; set; } = 1;
    public bool QuarantineOnDeath { get; set; } = true;
    public int MaxLevel { get; set; } = 5;
    public string QuarantineLoc { get; set; } = "0x02FA0100 -2.282979 0.158116 0.517504 -0.900291 0.000000 0.000000 0.435289";
    public List<string> Items { get; set; } = new()
    {
        "306 1",
        "300 50",
    };
}
