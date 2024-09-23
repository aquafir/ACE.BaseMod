namespace PlayerSave.Helpers;

public static class CharacterHelpers
{
    public static Character CopyCharacter(this Character original, uint accountId, string name) =>
        original.CopyCharacterAs(accountId, name, LoadOptions.Default);

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


    public static void Write(this Character character, BinaryWriter writer)
    {
        #region Shallow
        writer.Write(character.Id);
        writer.Write(character.AccountId);
        writer.Write(character.Name);
        writer.Write(character.IsPlussed);
        writer.Write(character.IsDeleted);
        writer.Write(character.DeleteTime);
        writer.Write(character.LastLoginTimestamp);
        writer.Write(character.TotalLogins);
        writer.Write(character.CharacterOptions1);
        writer.Write(character.CharacterOptions2);
        writer.Write(character.GameplayOptions.Length);
        writer.Write(character.GameplayOptions);
        writer.Write(character.SpellbookFilters);
        writer.Write(character.HairTexture);
        writer.Write(character.DefaultHairTexture);
        #endregion

        //character.BiotaPropertiesAllegiance.Write(writer);
        character.CharacterPropertiesContractRegistry.Write(writer);
        character.CharacterPropertiesFillCompBook.Write(writer);
        character.CharacterPropertiesFriendList.Write(writer);
        character.CharacterPropertiesQuestRegistry.Write(writer);
        character.CharacterPropertiesShortcutBar.Write(writer);
        character.CharacterPropertiesSpellBar.Write(writer);
        character.CharacterPropertiesSquelch.Write(writer);
        character.CharacterPropertiesTitleBook.Write(writer);
    }
    public static void ReadCharacter(this Character character, BinaryReader reader)
    {
        #region Shallow
        character.Id = reader.ReadUInt32();
        character.AccountId = reader.ReadUInt32();
        character.Name = reader.ReadString();
        character.IsPlussed = reader.ReadBoolean();
        character.IsDeleted = reader.ReadBoolean();
        character.DeleteTime = reader.ReadUInt64();
        character.LastLoginTimestamp = reader.ReadDouble();
        character.TotalLogins = reader.ReadInt32();
        character.CharacterOptions1 = reader.ReadInt32();
        character.CharacterOptions2 = reader.ReadInt32();
        character.GameplayOptions = reader.ReadBytes(reader.ReadInt32());
        character.SpellbookFilters = reader.ReadUInt32();
        character.HairTexture = reader.ReadUInt32();
        character.DefaultHairTexture = reader.ReadUInt32();
        #endregion

        character.ReadContracts(reader);
        character.ReadFillComps(reader);
        character.ReadFriends(reader);
        character.ReadQuests(reader);
        character.ReadShortcuts(reader);
        character.ReadSpellbars(reader);
        character.ReadSquelchs(reader);
        character.ReadTitles(reader);
    }

    //public static void Write(this ICollection<BiotaPropertiesAllegiance> props, BinaryWriter writer)
    //{
    //    writer.Write(props.Count);
    //    foreach (var prop in props)
    //    {
    //        //writer.Write(prop.Allegiance);
    //        writer.Write(prop.AllegianceId);
    //        writer.Write(prop.ApprovedVassal);
    //        writer.Write(prop.Banned);
    //        //writer.Write(prop.Character);
    //        writer.Write(prop.CharacterId);
    //    }
    //}
    //public static void ReadAllegiance(this Character character, BinaryReader reader)
    //{
    //    //Todo
    //}
    public static void Write(this ICollection<CharacterPropertiesContractRegistry> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            //Todo: decide about Character reference
            writer.Write(prop.CharacterId);
            writer.Write(prop.ContractId);
            writer.Write(prop.DeleteContract);
            writer.Write(prop.SetAsDisplayContract);
        }
    }
    public static void ReadContracts(this Character character, BinaryReader reader)
    {
        //Todo: decide about assuming clear?
        var props = character.CharacterPropertiesContractRegistry;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesContractRegistry
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                ContractId = reader.ReadUInt32(),
                DeleteContract = reader.ReadBoolean(),
                SetAsDisplayContract = reader.ReadBoolean(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesFillCompBook> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.QuantityToRebuy);
            writer.Write(prop.SpellComponentId);
        }
    }
    public static void ReadFillComps(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesFillCompBook;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesFillCompBook
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                QuantityToRebuy = reader.ReadInt32(),
                SpellComponentId = reader.ReadInt32(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesFriendList> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.FriendId);
        }
    }
    public static void ReadFriends(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesFriendList;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesFriendList
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                FriendId = reader.ReadUInt32(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesQuestRegistry> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.LastTimeCompleted);
            writer.Write(prop.NumTimesCompleted);

            writer.Write(prop.QuestName != null);
            if (prop.QuestName != null)
                writer.Write(prop.QuestName);
        }
    }
    public static void ReadQuests(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesQuestRegistry;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            var value = new CharacterPropertiesQuestRegistry
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                LastTimeCompleted = reader.ReadUInt32(),
                NumTimesCompleted = reader.ReadInt32(),
            };
            if (reader.ReadBoolean())
                value.QuestName = reader.ReadString();

            props.Add(value);
        }
    }
    public static void Write(this ICollection<CharacterPropertiesShortcutBar> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.ShortcutBarIndex);
            writer.Write(prop.ShortcutObjectId);
        }
    }
    public static void ReadShortcuts(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesShortcutBar;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesShortcutBar
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                ShortcutBarIndex = reader.ReadUInt32(),
                ShortcutObjectId = reader.ReadUInt32(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesSpellBar> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.SpellBarIndex);
            writer.Write(prop.SpellBarNumber);
            writer.Write(prop.SpellId);
        }
    }
    public static void ReadSpellbars(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesSpellBar;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesSpellBar
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                SpellBarIndex = reader.ReadUInt32(),
                SpellBarNumber = reader.ReadUInt32(),
                SpellId = reader.ReadUInt32(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesSquelch> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.SquelchAccountId);
            writer.Write(prop.SquelchCharacterId);
            writer.Write(prop.Type);
        }
    }
    public static void ReadSquelchs(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesSquelch;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesSquelch
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                SquelchAccountId = reader.ReadUInt32(),
                SquelchCharacterId = reader.ReadUInt32(),
                Type = reader.ReadUInt32(),
            });
        }
    }
    public static void Write(this ICollection<CharacterPropertiesTitleBook> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write(prop.CharacterId);
            writer.Write(prop.TitleId);
        }
    }
    public static void ReadTitles(this Character character, BinaryReader reader)
    {
        var props = character.CharacterPropertiesTitleBook;
        props.Clear();
        var count = reader.ReadInt32();

        for (var i = 0; i < count; i++)
        {
            props.Add(new CharacterPropertiesTitleBook
            {
                Character = character,
                CharacterId = reader.ReadUInt32(),
                TitleId = reader.ReadUInt32(),
            });
        }
    }
}