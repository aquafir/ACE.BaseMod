using ACE.Entity.Adapter;
using ACE.Shared.Augments;

namespace ACE.Shared.Tests;

public class AugmentTests
{
    IEnumerable<Weenie> content;
    FakeWorldObject wo;
    Weenie weenie;
    Entity.Models.Biota biota;

    [SetUp]
    public void Setup()
    {
        var content = @"C:\ACE\Server\Content\json\weenies\35394 - BloodScorch.json";
        ContentHelpers.TryLoadTemplate(content, out weenie);
        biota = weenie.ConvertToBiota(0);
        //var weenie = content.Where(x => x.WeenieType == WeenieType.Creature).FirstOrDefault();
        wo = new(biota);
    }

    [Test]
    public void CreateWorldObject()
    {
        Assert.NotNull(wo); 
    }

    [Test]
    public void SetPropertyString()
    {
        wo.SetProperty(PropertyString.Name, "Test");
        Assert.AreEqual(wo.Name, "Test");
    }

    [Test]
    public void SetPropertyInt()
    {
        wo.SetProperty(PropertyInt.EncumbranceVal, 5000);
        Assert.AreEqual(wo.EncumbranceVal, 5000);
    }


    [Test]
    public void SetPropertyAugment()
    {
        Assert.True(wo.TryAugment(AugmentType.Int, (int)PropertyInt.EncumbranceVal, Operation.Assign, 5000));
        Assert.AreEqual(wo.EncumbranceVal, 5000);
    }


}


public class FakeWorldObject : WorldObject
{
    public FakeWorldObject(Weenie weenie, ObjectGuid guid) : base(weenie, guid)
    {
        SetEphemeralValues();
    }

    /// <summary>
    /// Restore a WorldObject from the database.
    /// </summary>
    public FakeWorldObject(Entity.Models.Biota biota) : base(biota)
    {
        SetEphemeralValues();
    }

    private void SetEphemeralValues()
    {
    }

}