using ACE.DatLoader.Entity;
using ACE.DatLoader.FileTypes;

namespace ACE.Shared.Helpers;
//ClothingTable
//  ClothingBaseEffectEx (Dic)
//      CloObjectEffect (List)
//          Index
//          ModelId
//          CloTextureEffect (List)
//              OldTexture
//              NewTexture
//  CloSubPalEffectEx (Dic)
//      Icon
//      CloSubPalette (List)
//          CloSubPaletteRange (List)
//              Offset
//              NumColors
//public class ClothingTableEx : ClothingTable
//{
//    public new Dictionary<uint, ClothingBaseEffectEx> ClothingBaseEffects = new();
//    public new Dictionary<uint, CloSubPalEffectEx> ClothingSubPalEffects = new();

//    public ClothingTable Convert()
//    {
//        ClothingTable value = new();
//        foreach (var cbe in ClothingBaseEffects)
//            value.ClothingBaseEffects.Add(cbe.Key, cbe.Value.Convert());

//        foreach (var cbe in ClothingSubPalEffects)
//            value.ClothingSubPalEffects.Add(cbe.Key, cbe.Value.Convert());

//        return value;
//    }
//}

//public class CloSubPalEffectEx : CloSubPalEffect
//{
//    public new uint Icon;
//    public new List<CloSubPaletteEx> CloSubPalettes = new();

//    public CloSubPalEffect Convert()
//    {
//        CloSubPalEffect value = new();
//        //Traverse.Create(value).Property(nameof(value.Icon)).SetValue(value.Icon);
//        value.Icon = Icon;
//        value.CloSubPalettes.AddRange(CloSubPalettes.ConvertAll(x => x.Convert()));
//        return value;
//    }
//}

//public class CloSubPaletteEx : CloSubPalette
//{
//    public new List<CloSubPaletteRangeEx> Ranges = new();
//    public new uint PaletteSet;

//    public CloSubPalette Convert()
//    {
//        CloSubPalette value = new();
//        value.PaletteSet = PaletteSet;
//        value.Ranges.AddRange(Ranges);
//        return value;
//    }
//}

//public class CloSubPaletteRangeEx : CloSubPaletteRange
//{
//    public new uint Offset;
//    public new uint NumColors;

//    public CloSubPaletteRange Convert()
//    {
//        CloSubPaletteRange value = new();
//        value.Offset = Offset;
//        value.NumColors = NumColors;
//        return value;
//    }
//}

//public class ClothingBaseEffectEx : ClothingBaseEffect
//{
//    public new List<CloObjectEffectEx> CloObjectEffects = new();

//    public ClothingBaseEffect Convert()
//    {
//        ClothingBaseEffect value = new();
//        var converted = CloObjectEffects.ConvertAll(x => x.Convert());
//        value.CloObjectEffects.AddRange(CloObjectEffects.ConvertAll(x => x.Convert()));
//        return value;
//    }
//}

//public class CloObjectEffectEx : CloObjectEffect
//{
//    public new uint Index;
//    public new uint ModelId;
//    public new List<CloTextureEffectEx> CloTextureEffects = new();

//    public CloObjectEffect Convert()
//    {
//        CloObjectEffect value = new();
//        value.Index = Index;
//        value.ModelId = ModelId;
//        //Traverse.Create(value).Property(nameof(value.Index)).SetValue(value.Index);
//        //Traverse.Create(value).Property(nameof(value.ModelId)).SetValue(value.ModelId);
//        value.CloTextureEffects.AddRange(CloTextureEffects);
//        return value;
//    }
//}

//public class CloTextureEffectEx : CloTextureEffect
//{
//    public new uint OldTexture;
//    public new uint NewTexture;

//    public CloTextureEffect Convert()
//    {
//        CloTextureEffect value = new();
//        value.OldTexture = OldTexture;
//        value.NewTexture = NewTexture;
//        //Traverse.Create(value).Property(nameof(value.OldTexture)).SetValue(value.OldTexture);
//        //Traverse.Create(value).Property(nameof(value.NewTexture)).SetValue(value.NewTexture);
//        return value;
//    }
//}

public class ClothingTableEx : ClothingTable
{
    public new Dictionary<uint, ClothingBaseEffectEx> ClothingBaseEffects = new();
    public new Dictionary<uint, CloSubPalEffectEx> ClothingSubPalEffects = new();

    public ClothingTable Convert()
    {
        ClothingTable value = new();
        foreach (var cbe in ClothingBaseEffects)
            value.ClothingBaseEffects.Add(cbe.Key, cbe.Value.Convert());

        foreach (var cbe in ClothingSubPalEffects)
            value.ClothingSubPalEffects.Add(cbe.Key, cbe.Value.Convert());

        return value;
    }
}

public class CloSubPalEffectEx : CloSubPalEffect
{
    public new int Icon;
    public new List<CloSubPaletteEx> CloSubPalettes = new();

    public CloSubPalEffect Convert()
    {
        CloSubPalEffect value = new();
        value.Icon = (uint)Icon;
        value.CloSubPalettes.AddRange(CloSubPalettes.ConvertAll(x => x.Convert()));
        return value;
    }
}

public class CloSubPaletteEx : CloSubPalette
{
    public new List<CloSubPaletteRangeEx> Ranges = new();
    public new int PaletteSet;

    public CloSubPalette Convert()
    {
        CloSubPalette value = new();
        value.PaletteSet = (uint)PaletteSet;
        value.Ranges.AddRange(Ranges);
        return value;
    }
}

public class CloSubPaletteRangeEx : CloSubPaletteRange
{
    public new int Offset;
    public new int NumColors;

    public CloSubPaletteRange Convert()
    {
        CloSubPaletteRange value = new();
        value.Offset = (uint)Offset;
        value.NumColors = (uint)NumColors;
        return value;
    }
}

public class ClothingBaseEffectEx : ClothingBaseEffect
{
    public new List<CloObjectEffectEx> CloObjectEffects = new();

    public ClothingBaseEffect Convert()
    {
        ClothingBaseEffect value = new();
        var converted = CloObjectEffects.ConvertAll(x => x.Convert());
        value.CloObjectEffects.AddRange(CloObjectEffects.ConvertAll(x => x.Convert()));
        return value;
    }
}

public class CloObjectEffectEx : CloObjectEffect
{
    public new int Index;
    public new int ModelId;
    public new List<CloTextureEffectEx> CloTextureEffects = new();

    public CloObjectEffect Convert()
    {
        CloObjectEffect value = new();
        value.Index = (uint)Index;
        value.ModelId = (uint)ModelId;
        value.CloTextureEffects.AddRange(CloTextureEffects);
        return value;
    }
}

public class CloTextureEffectEx : CloTextureEffect
{
    public new int OldTexture;
    public new int NewTexture;

    public CloTextureEffect Convert()
    {
        CloTextureEffect value = new();
        value.OldTexture = (uint)OldTexture;
        value.NewTexture = (uint)NewTexture;
        return value;
    }
}



public static class ClothingTableExtensions
{
    public static ClothingTable Clone(this ClothingTable table)
        => table.ToClothingTableEx().Convert();

    public static ClothingTableEx ToClothingTableEx(this ClothingTable table)
    {
        JsonSerializerOptions options = new()
        {
            IncludeFields = true
        };
        var json = JsonSerializer.Serialize(table, options);
        return JsonSerializer.Deserialize<ClothingTableEx>(json, options);
    }
}