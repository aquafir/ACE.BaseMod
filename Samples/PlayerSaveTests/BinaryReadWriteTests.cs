using ACE.Database.Models.Shard;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlayerSave;
using PlayerSave.Helpers;
using System.Text.Json;

namespace PlayerSaveTests;

[TestClass()]
public class BiotaHelpersTests
{
    private static Biota _biota;
    private static Character _character;
    private static List<Biota> _inventory;
    private static List<Biota> _wielded;

    [ClassInitialize()]
    public static void ClassInitialize(TestContext context)
    {
        var savePath = @"C:\ACE\Mods\PlayerSave\Saves\Void.acesave";

        var data = File.ReadAllBytes(savePath);
        var text = data.GZipToString();
        var save = JsonSerializer.Deserialize<PlayerSave.PlayerSave>(text);

        _biota = save.PlayerBiota;
        _character = save.Character;
        _inventory = save.Inventory;
        _wielded = save.Wielded;
    }

    [TestMethod()]
    public void WriteReadBiota()
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        using BinaryReader reader = new(ms);
        var biota = new Biota();

        _biota.Write(writer);

        ms.Seek(0, SeekOrigin.Begin);
        biota.ReadBiota(reader);

        //Simple check for fully read
        Assert.IsTrue(writer.BaseStream.Length == writer.BaseStream.Position);
    }

    [TestMethod()]
    public void WriteReadCharacter()
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        using BinaryReader reader = new(ms);
        var character = new Character();

        _character.Write(writer);

        ms.Seek(0, SeekOrigin.Begin);

        character.ReadCharacter(reader);

        Assert.IsTrue(writer.BaseStream.Length == writer.BaseStream.Position);
    }


    [TestMethod()]
    public void WriteReadBiotas()
    {
        using MemoryStream ms = new();
        using BinaryWriter writer = new(ms);
        using BinaryReader reader = new(ms);
        var inventory = new List<Biota>();
        var wielded = new List<Biota>();

        _inventory.Write(writer);
        _wielded.Write(writer);

        ms.Seek(0, SeekOrigin.Begin);

        inventory.ReadBiotas(reader);
        wielded.ReadBiotas(reader);

        Assert.IsTrue(writer.BaseStream.Length == writer.BaseStream.Position);
    }
}