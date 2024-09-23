namespace PlayerSave;

public static class SnapshotHelpers
{
    //public static string SerializePlayer(this Player player)
    //{
    //    var state = new StringBuilder();

    //    var biota = player.Biota;

    //    string returnState = "1=";
    //    returnState += $"{DateTime.UtcNow}=";

    //    // need level 25, available skill credits 24
    //    returnState += $"24={player.AvailableSkillCredits}=25={player.Level}=";

    //    // need total xp 1, unassigned xp 2
    //    returnState += $"1={player.TotalExperience}=2={player.AvailableExperience}=";

    //    // need all attributes
    //    // 1 through 6 str, end, coord, quick, focus, self
    //    foreach (var kvp in biota.PropertiesAttribute)
    //    {
    //        var att = kvp.Value;

    //        if (kvp.Key > 0 && (int)kvp.Key <= 6)
    //        {
    //            returnState += $"{(int)kvp.Key}=";
    //            returnState += $"{att.InitLevel}=";
    //            returnState += $"{att.LevelFromCP}=";
    //            returnState += $"{att.CPSpent}=";
    //        }
    //    }

    //    // need all vitals
    //    // 1, 3, 5 H,S,M (2,4,6 are current values and are not stored since they will be maxed entering/exiting godmode)
    //    foreach (var kvp in biota.PropertiesAttribute2nd)
    //    {
    //        var attSec = kvp.Value;

    //        if ((int)kvp.Key == 1 || (int)kvp.Key == 3 || (int)kvp.Key == 5)
    //        {
    //            returnState += $"{(int)kvp.Key}=";
    //            returnState += $"{attSec.InitLevel}=";
    //            returnState += $"{attSec.LevelFromCP}=";
    //            returnState += $"{attSec.CPSpent}=";
    //            returnState += $"{attSec.CurrentLevel}=";
    //        }
    //    }

    //    // need all skills
    //    foreach (var kvp in biota.PropertiesSkill)
    //    {
    //        var sk = kvp.Value;

    //        if (SkillHelper.ValidSkills.Contains(kvp.Key))
    //        {
    //            returnState += $"{(int)kvp.Key}=";
    //            returnState += $"{sk.LevelFromPP}=";
    //            returnState += $"{(uint)sk.SAC}=";
    //            returnState += $"{sk.PP}=";
    //            returnState += $"{sk.InitLevel}=";
    //        }
    //    }

    //    // Check string is correctly formatted before altering stats
    //    // correctly formatted return string should have 240 entries
    //    // if the construction of the string changes - this will need to be updated to match
    //    if (returnState.Split("=").Length != 240)
    //    {
    //        ChatPacket.SendServerMessage(session, "Godmode is not available at this time.", ChatMessageType.Broadcast);
    //        Console.WriteLine($"Player {player.Name} tried to enter god mode but there was an error with the godString length. (length = {returnState.Split("=").Length}) Godmode not available.");
    //        return;
    //    }

    //    // save return state to db in property string
    //    player.SetProperty(PropertyString.GodState, returnState);
    //    player.SaveBiotaToDatabase();
    //}


    //    return state.ToString();
    //}
}