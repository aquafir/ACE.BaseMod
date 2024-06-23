using System;
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
}
