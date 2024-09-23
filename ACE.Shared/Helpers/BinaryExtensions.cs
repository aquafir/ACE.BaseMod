namespace ACE.Shared.Helpers;
public static class BinaryExtensions
{
    //private static uint CalculatePadMultipleM(uint length, uint multiple) { return multiple * ((length + multiple - 1u) / multiple) - length; }

    //public static void WriteString16LM(this BinaryWriter writer, string data)
    //{
    //    if (data == null) data = "";

    //    writer.Write((ushort)data.Length);
    //    writer.Write(System.Text.Encoding.GetEncoding(1252).GetBytes(data));

    //    // client expects string length to be a multiple of 4 including the 2 bytes for length
    //    writer.Pad(CalculatePadMultipleM(sizeof(ushort) + (uint)data.Length, 4u));
    //}

    //public static void WritePackedDwordM(this BinaryWriter writer, uint value)
    //{
    //    if (value <= 32767)
    //    {
    //        ushort networkValue = Convert.ToUInt16(value);
    //        writer.Write(BitConverter.GetBytes(networkValue));
    //    }
    //    else
    //    {
    //        uint packedValue = (value << 16) | ((value >> 16) | 0x8000);
    //        writer.Write(BitConverter.GetBytes(packedValue));
    //    }
    //}

    //public static void WritePackedDwordOfKnownTypeM(this BinaryWriter writer, uint value, uint type)
    //{
    //    if ((value & type) > 0)
    //        value -= type;

    //    writer.WritePackedDword(value);
    //}

    //public static void WriteUInt16BEM(this BinaryWriter writer, ushort value)
    //{
    //    ushort beValue = (ushort)((ushort)((value & 0xFF) << 8) | ((value >> 8) & 0xFF));
    //    writer.Write(beValue);
    //}

    //public static void PadM(this BinaryWriter writer, uint pad) { writer.Write(new byte[pad]); }

    //public static void PadM(this BinaryReader reader, uint pad) { reader.ReadBytes((int)pad); }

    //public static void AlignM(this BinaryWriter writer)
    //{
    //    writer.Pad(CalculatePadMultiple((uint)writer.BaseStream.Length, 4u));
    //}

    //public static void AlignM(this BinaryReader reader)
    //{
    //    reader.Pad(CalculatePadMultiple((uint)reader.BaseStream.Position, 4u));
    //}

    //public static string BuildPacketStringM(this byte[] bytes, int startPosition = 0, int bytesToOutput = 9999)
    //{
    //    TextWriter tw = new StringWriter();
    //    byte[] buffer = bytes;

    //    int column = 0;
    //    int row = 0;
    //    int columns = 16;
    //    tw.Write("   x  ");
    //    for (int i = 0; i < columns; i++)
    //    {
    //        tw.Write(i.ToString().PadLeft(3));
    //    }
    //    tw.WriteLine("  |Text");
    //    tw.Write("   0  ");

    //    string asciiLine = "";
    //    for (int i = startPosition; i < startPosition + bytesToOutput; i++)
    //    {
    //        if (i >= buffer.Length)
    //        {
    //            break;
    //        }
    //        if (column >= columns)
    //        {
    //            row++;
    //            column = 0;
    //            tw.WriteLine("  |" + asciiLine);
    //            asciiLine = "";
    //            tw.Write((row * columns).ToString().PadLeft(4));
    //            tw.Write("  ");
    //        }

    //        tw.Write(buffer[i].ToString("X2").PadLeft(3));

    //        if (Char.IsControl((char)buffer[i]))
    //            asciiLine += " ";
    //        else
    //            asciiLine += (char)buffer[i];
    //        column++;
    //    }

    //    tw.Write("".PadLeft((columns - column) * 3));
    //    tw.WriteLine("  |" + asciiLine);
    //    return tw.ToString();
    //}

    //public static void WritePositionM(this BinaryWriter writer, uint value, long position)
    //{
    //    long originalPosition = writer.BaseStream.Position;
    //    writer.BaseStream.Position = position;
    //    writer.Write(value);
    //    writer.BaseStream.Position = originalPosition;
    //}
}
