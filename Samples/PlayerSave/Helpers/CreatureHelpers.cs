namespace PlayerSave.Helpers;

public static class CreatureHelpers
{
    /// <summary>
    /// Write CreatureSkills to a binary stream
    /// </summary>
    public static void Write(this Dictionary<Skill, CreatureSkill> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write((uint)prop.Value.AdvancementClass);
            writer.Write(prop.Value.ExperienceSpent);
            writer.Write(prop.Value.InitLevel);
            writer.Write(prop.Value.Ranks);
        }
    }
    /// <summary>
    /// Read CreatureSkills into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadSkills(this Dictionary<Skill, CreatureSkill> props, BinaryReader reader, Creature creature = null)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (Skill)reader.ReadUInt16();

            var sac = (SkillAdvancementClass)reader.ReadUInt32();
            var experienceSpent = reader.ReadUInt32();
            var initLevel = reader.ReadUInt32();
            var ranks = reader.ReadUInt16();

            if (props.ContainsKey(key))
            {
                props[key].AdvancementClass = sac;
                props[key].ExperienceSpent = experienceSpent;
                props[key].InitLevel = initLevel;
                props[key].Ranks = ranks;
            }
            else
            {
                if (creature is null)
                    throw new MissingFieldException("No Creature to assign the skill to");

                //Todo: This requires a creature
                CreatureSkill skill = new CreatureSkill(creature, key, new PropertiesSkill()
                {
                    SAC = sac,
                    //Bonus
                    InitLevel = initLevel,
                    //Ranks from xp spent
                    LevelFromPP = ranks,
                    //Experience spent
                    PP = experienceSpent
                });
                props.Add(key, skill);
            }
        }
    }
    public static void ReadSkills(this Creature creature, BinaryReader reader) => creature.Skills.ReadSkills(reader, creature);

    /// <summary>
    /// Write CreatureAttributes to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyAttribute, CreatureAttribute> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            //Values written the ones set by /god
            writer.Write((ushort)prop.Key);
            //writer.Write(prop.Value.Current);
            writer.Write(prop.Value.ExperienceSpent);
            //writer.Write(prop.Value.PartialRegen);
            writer.Write(prop.Value.Ranks);
            //writer.Write(prop.Value.RegenRate);
            writer.Write(prop.Value.StartingValue);
        }
    }
    /// <summary>
    /// Read CreatureAttributes into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyAttributes(this Dictionary<PropertyAttribute, CreatureAttribute> props, BinaryReader reader, Creature creature = null)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyAttribute)reader.ReadUInt16();

            var experienceSpent = reader.ReadUInt32();
            var ranks = reader.ReadUInt32();
            var startingValue = reader.ReadUInt32();

            if (props.ContainsKey(key))
            {
                props[key].ExperienceSpent = experienceSpent;
                props[key].Ranks = ranks;
                props[key].StartingValue = startingValue;
            }
            else
            {
                //Todo: This requires a Creature to assign to
                if (creature is null)
                    throw new MissingFieldException("No Creature to assign the skill to");

                CreatureAttribute attribute = new(creature, key)
                {
                    ExperienceSpent = experienceSpent,
                    Ranks = ranks,
                    StartingValue = startingValue,
                };

                props.Add(key, attribute);
            }
        }
    }
    public static void ReadPropertyAttributes(this Creature creature, BinaryReader reader) =>
creature.Attributes.ReadPropertyAttributes(reader, creature);

    /// <summary>
    /// Write CreatureVitals to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyAttribute2nd, CreatureVital> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            //Values written the ones set by /god
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value.Current);
            writer.Write(prop.Value.ExperienceSpent);
            //writer.Write(prop.Value.PartialRegen);
            writer.Write(prop.Value.Ranks);
            //writer.Write(prop.Value.RegenRate);
            writer.Write(prop.Value.StartingValue);
        }
    }
    /// <summary>
    /// Read CreatureVitals into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyAttribute2nds(this Dictionary<PropertyAttribute2nd, CreatureVital> props, BinaryReader reader, Creature creature = null)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyAttribute2nd)reader.ReadUInt16();

            var experienceSpent = reader.ReadUInt32();
            var ranks = reader.ReadUInt32();
            var startingValue = reader.ReadUInt32();

            if (props.ContainsKey(key))
            {
                props[key].ExperienceSpent = experienceSpent;
                props[key].Ranks = ranks;
                props[key].StartingValue = startingValue;
            }
            else
            {
                //Todo: This requires a Creature to assign to
                if (creature is null)
                    throw new MissingFieldException("No Creature to assign the skill to");

                CreatureVital attribute = new(creature, key)
                {
                    ExperienceSpent = experienceSpent,
                    Ranks = ranks,
                    StartingValue = startingValue,
                };

                props.Add(key, attribute);
            }
        }
    }
    public static void ReadPropertyAttribute2nds(this Creature creature, BinaryReader reader) =>
        creature.Vitals.ReadPropertyAttribute2nds(reader, creature);
}
