namespace AccessDb
{
    public class PlayerSaveOptions
    {
        //Character options
        public bool IncludeContracts { get; internal set; } = true;
        public bool IncludeTitles { get; internal set; } = true;
        public bool IncludeSquelch { get; internal set; } = true;
        public bool IncludeSpellbar { get; internal set; } = true;
        public bool IncludeShortcuts { get; internal set; } = true;
        public bool IncludeQuests { get; internal set; } = true;
        public bool IncludeFriendList { get; internal set; } = true;
        public bool IncludeFillComp { get; internal set; } = true;
        public bool IncludeInventory { get; internal set; }
        public bool IncludeWielded { get; internal set; }

        //Biota options



        //Possession options



        //Todo: may remove and just use initial values for default
        public static PlayerSaveOptions Default => new PlayerSaveOptions
        {           
            IncludeContracts =      true,
            IncludeTitles =         true,
            IncludeSquelch =        true,
            IncludeSpellbar =       true,
            IncludeShortcuts =      true,
            IncludeQuests =         true,
            IncludeFriendList =     true,
            IncludeFillComp =       true,
            IncludeInventory =      false,
            IncludeWielded =        true,
        };

    }
}