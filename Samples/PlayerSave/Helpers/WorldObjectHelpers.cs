namespace PlayerSave.Helpers;

public static class WorldObjectHelpers
{
    /// <summary>
    /// Write PropertyBools to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyBool, bool> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyBools into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyBools(this Dictionary<PropertyBool, bool> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyBool)reader.ReadUInt16();
            var value = reader.ReadBoolean();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyDIDs to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyDataId, uint> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyDIDs into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyDataIDs(this Dictionary<PropertyDataId, uint> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyDataId)reader.ReadUInt16();
            var value = reader.ReadUInt32();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyFloats to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyFloat, double> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyFloats into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyFloats(this Dictionary<PropertyFloat, double> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyFloat)reader.ReadUInt16();
            var value = reader.ReadDouble();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyIIDs to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyInstanceId, uint> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyIIDs into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyInstanceIDs(this Dictionary<PropertyInstanceId, uint> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyInstanceId)reader.ReadUInt16();
            var value = reader.ReadUInt32();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyInts to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyInt, int> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyInts into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyInts(this Dictionary<PropertyInt, int> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyInt)reader.ReadUInt16();
            var value = reader.ReadInt32();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyInt64s to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyInt64, long> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyInt64s into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyInt64s(this Dictionary<PropertyInt64, long> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyInt64)reader.ReadUInt16();
            var value = reader.ReadInt64();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }
    /// <summary>
    /// Write PropertyStrings to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PropertyString, string> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value);
        }
    }
    /// <summary>
    /// Read PropertyStrings into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPropertyStrings(this Dictionary<PropertyString, string> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PropertyString)reader.ReadUInt16();
            var value = reader.ReadString();
            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }

    /// <summary>
    /// Write Positions to a binary stream
    /// </summary>
    public static void Write(this Dictionary<PositionType, Position> props, BinaryWriter writer)
    {
        writer.Write(props.Count);
        foreach (var prop in props)
        {
            writer.Write((ushort)prop.Key);
            writer.Write(prop.Value.LandblockId.Raw);
            writer.Write(prop.Value.PositionX);
            writer.Write(prop.Value.PositionY);
            writer.Write(prop.Value.PositionZ);
            writer.Write(prop.Value.RotationX);
            writer.Write(prop.Value.RotationY);
            writer.Write(prop.Value.RotationZ);
            writer.Write(prop.Value.RotationW);
        }
    }
    /// <summary>
    /// Read Positions into a dictionary, adding them if they don't exist, to a number specified by the first int of the reader's stream
    /// </summary>
    public static void ReadPositions(this Dictionary<PositionType, Position> props, BinaryReader reader)
    {
        for (var i = 0; i < reader.ReadInt32(); i++)
        {
            var key = (PositionType)reader.ReadUInt16();

            var value = new Position(reader.ReadUInt32(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            if (props.ContainsKey(key))
                props[key] = value;
            else
                props.Add(key, value);
        }
    }

    /// <summary>
    /// Writes non-ephemeral PropertyBools
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyBool, bool> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesBool.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral DataIDs
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyDataId, uint> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesDataId.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral PropertyFloats
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyFloat, double> props, BinaryWriter writer) =>
            props.Where(p => !EphemeralProperties.PropertiesDouble.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral InstanceIDs
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyInstanceId, uint> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesInstanceId.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral PropertyInts
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyInt, int> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesInt.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral PropertyInt64s
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyInt64, long> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesInt64.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);
    /// <summary>
    /// Writes non-ephemeral PropertyStrings
    /// </summary>
    public static void WriteNonEphemeral(this Dictionary<PropertyString, string> props, BinaryWriter writer) =>
        props.Where(p => !EphemeralProperties.PropertiesString.Contains(p.Key)).ToDictionary(k => k.Key, k => k.Value).Write(writer);


    public static PlayerSnapshot ReadSnapshot(this BinaryReader reader)
    {
        //reader.
        PlayerSnapshot snapshot = new();

        snapshot.Bools = new Dictionary<PropertyBool, bool>();

        return snapshot;
    }



    private static HashSet<T> GetNonEmphemeral<T>()
    {
        var list = typeof(T).GetFields().Select(x => new
        {
            att = x.GetCustomAttributes(false).OfType<EphemeralAttribute>().FirstOrDefault(),
            member = x
        }).Where(x => x.att is null && x.member.Name != "value__").Select(x => (T)x.member.GetValue(null)).ToList();

        return new HashSet<T>(list);
    }
}
