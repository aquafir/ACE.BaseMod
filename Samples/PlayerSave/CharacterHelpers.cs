using ACE.Database.Models.Shard;
using ACE.Server.Managers;

namespace PlayerSave;

public static class CharacterHelpers
{
    public static Character CopyCharacter(this Character original, uint accountId, string name) =>
        CopyCharacterAs(original, accountId, name, LoadOptions.Default);

    /// <summary>
    /// Create a copy of a Character with a give name and account ID and update chosen properties with Character's GUID
    /// </summary>
    public static Character CopyCharacterAs(this Character original, uint accountId, string name, LoadOptions options)
    {
        var newCharacter = original.CopyCharacterShallow(accountId, name);

        if (options.IncludeContracts)
            newCharacter.CharacterPropertiesContractRegistry = original.CopyContractsAs(newCharacter.Id);

        if (options.IncludeFillComp)
            newCharacter.CharacterPropertiesFillCompBook = original.CopyFillCompAs(newCharacter.Id);

        if (options.IncludeFriendList)
            newCharacter.CharacterPropertiesFriendList = original.CopyFriendListAs(newCharacter.Id);

        if (options.IncludeQuests)
            newCharacter.CharacterPropertiesQuestRegistry = original.CopyQuestsAs(newCharacter.Id);

        if (options.IncludeShortcuts)
            newCharacter.CharacterPropertiesShortcutBar = original.CopyShortcutsAs(newCharacter.Id);

        if (options.IncludeSpellbar)
            newCharacter.CharacterPropertiesSpellBar = original.CopySpellbarAs(newCharacter.Id);

        if (options.IncludeSquelch)
            newCharacter.CharacterPropertiesSquelch = original.CopySquelchAs(newCharacter.Id);

        if (options.IncludeTitles)
            newCharacter.CharacterPropertiesTitleBook = original.CopyTitlesAs(newCharacter.Id);

        return newCharacter;
    }

    public static Character CopyCharacterShallow(this Character character, uint accountId, string name = "")
    {
        var copy = new Character
        {
            Id = GuidManager.NewPlayerGuid().Full,
            AccountId = accountId,
            Name = name,
            CharacterOptions1 = character.CharacterOptions1,
            CharacterOptions2 = character.CharacterOptions2,
            DefaultHairTexture = character.DefaultHairTexture,
            GameplayOptions = character.GameplayOptions,
            HairTexture = character.HairTexture,
            IsPlussed = character.IsPlussed,
            SpellbookFilters = character.SpellbookFilters,
            TotalLogins = 1 // existingCharacter.TotalLogins
        };

        return copy;
    }

    public static List<CharacterPropertiesContractRegistry> CopyContractsAs(this Character character, uint Id) =>
character.CharacterPropertiesContractRegistry.Select(entry =>
new CharacterPropertiesContractRegistry { CharacterId = Id, ContractId = entry.ContractId, DeleteContract = entry.DeleteContract, SetAsDisplayContract = entry.SetAsDisplayContract }).ToList();

    public static List<CharacterPropertiesFillCompBook> CopyFillCompAs(this Character character, uint Id) =>
        character.CharacterPropertiesFillCompBook.Select(entry =>
        new CharacterPropertiesFillCompBook { CharacterId = Id, QuantityToRebuy = entry.QuantityToRebuy, SpellComponentId = entry.SpellComponentId }).ToList();

    public static List<CharacterPropertiesFriendList> CopyFriendListAs(this Character character, uint Id) =>
        character.CharacterPropertiesFriendList.Select(entry =>
        new CharacterPropertiesFriendList { CharacterId = Id, FriendId = entry.FriendId }).ToList();

    public static List<CharacterPropertiesQuestRegistry> CopyQuestsAs(this Character character, uint Id) =>
        character.CharacterPropertiesQuestRegistry.Select(entry =>
        new CharacterPropertiesQuestRegistry { CharacterId = Id, LastTimeCompleted = entry.LastTimeCompleted, NumTimesCompleted = entry.NumTimesCompleted, QuestName = entry.QuestName }).ToList();

    public static List<CharacterPropertiesShortcutBar> CopyShortcutsAs(this Character character, uint Id) =>
        character.CharacterPropertiesShortcutBar.Select(entry =>
        new CharacterPropertiesShortcutBar { CharacterId = Id, ShortcutBarIndex = entry.ShortcutBarIndex, ShortcutObjectId = entry.ShortcutObjectId }).ToList();

    public static List<CharacterPropertiesSpellBar> CopySpellbarAs(this Character character, uint Id) =>
        character.CharacterPropertiesSpellBar.Select(entry =>
        new CharacterPropertiesSpellBar { CharacterId = Id, SpellBarIndex = entry.SpellBarIndex, SpellBarNumber = entry.SpellBarNumber, SpellId = entry.SpellId }).ToList();

    public static List<CharacterPropertiesSquelch> CopySquelchAs(this Character character, uint Id) =>
        character.CharacterPropertiesSquelch.Select(entry =>
        new CharacterPropertiesSquelch { CharacterId = Id, SquelchAccountId = entry.SquelchAccountId, SquelchCharacterId = entry.SquelchCharacterId, Type = entry.Type }).ToList();

    public static List<CharacterPropertiesTitleBook> CopyTitlesAs(this Character character, uint Id) =>
        character.CharacterPropertiesTitleBook.Select(entry =>
        new CharacterPropertiesTitleBook { CharacterId = Id, TitleId = entry.TitleId }).ToList();

    #region Non-LINQ/Lambda version
    //public static List<CharacterPropertiesContractRegistry> CopyContractsAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesContractRegistry> list = new();
    //    foreach (var entry in character.CharacterPropertiesContractRegistry)
    //        list.Add(new CharacterPropertiesContractRegistry { CharacterId = Id, ContractId = entry.ContractId, DeleteContract = entry.DeleteContract, SetAsDisplayContract = entry.SetAsDisplayContract }); 
    //    return list;
    //}

    //public static List<CharacterPropertiesFillCompBook> CopyFillCompAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesFillCompBook> list = new();
    //    foreach (var entry in character.CharacterPropertiesFillCompBook)
    //        list.Add(new CharacterPropertiesFillCompBook { CharacterId = Id, QuantityToRebuy = entry.QuantityToRebuy, SpellComponentId = entry.SpellComponentId });
    //    return list;
    //}

    //public static List<CharacterPropertiesFriendList> CopyFriendListAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesFriendList> list = new();
    //    foreach (var entry in character.CharacterPropertiesFriendList)
    //        list.Add(new CharacterPropertiesFriendList { CharacterId = Id, FriendId = entry.FriendId });
    //    return list;
    //}

    //public static List<CharacterPropertiesQuestRegistry> CopyQuestsAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesQuestRegistry> list = new();
    //    foreach (var entry in character.CharacterPropertiesQuestRegistry)
    //        list.Add(new CharacterPropertiesQuestRegistry { CharacterId = Id, LastTimeCompleted = entry.LastTimeCompleted, NumTimesCompleted = entry.NumTimesCompleted, QuestName = entry.QuestName });
    //    Debugger.Break();
    //    return list;
    //}

    //public static List<CharacterPropertiesShortcutBar> CopyShortcutsAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesShortcutBar> list = new();
    //    foreach (var entry in character.CharacterPropertiesShortcutBar)
    //        list.Add(new CharacterPropertiesShortcutBar { CharacterId = Id, ShortcutBarIndex = entry.ShortcutBarIndex, ShortcutObjectId = entry.ShortcutObjectId });
    //    return list;
    //}

    //public static List<CharacterPropertiesSpellBar> CopySpellbarAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesSpellBar> list = new();
    //    foreach (var entry in character.CharacterPropertiesSpellBar)
    //        list.Add(new CharacterPropertiesSpellBar { CharacterId = Id, SpellBarIndex = entry.SpellBarIndex, SpellBarNumber = entry.SpellBarNumber, SpellId = entry.SpellId });
    //    return list;
    //}

    //public static List<CharacterPropertiesSquelch> CopySquelchAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesSquelch> list = new();
    //    foreach (var entry in character.CharacterPropertiesSquelch)
    //        list.Add(new CharacterPropertiesSquelch { CharacterId = Id, SquelchAccountId = entry.SquelchAccountId, SquelchCharacterId = entry.SquelchCharacterId, Type = entry.Type });
    //    return list;
    //}

    //public static List<CharacterPropertiesTitleBook> CopyTitlesAs(this Character character, uint Id)
    //{
    //    List<CharacterPropertiesTitleBook> list = new();
    //    foreach (var entry in character.CharacterPropertiesTitleBook)
    //        list.Add(new CharacterPropertiesTitleBook { CharacterId = Id, TitleId = entry.TitleId });
    //    return list;
    //} 
    #endregion
}