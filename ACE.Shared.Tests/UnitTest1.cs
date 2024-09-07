using ACE.Entity.Adapter;

namespace ACE.Shared.Tests;

public class Tests
{
    IEnumerable<Weenie> content;
    Creature creature;

    [SetUp]
    public void Setup()
    {
        content = ContentHelpers.GetCustomWeenies(@"C:\ACE\Server\Content");
        var weenie = content.Where(x => x.WeenieType == WeenieType.Creature).FirstOrDefault();
        creature = new(weenie.ConvertToBiota(0));
    }

    [Test]
    public void CreateCreature()
    {
        Assert.NotNull(creature); 
    }
}