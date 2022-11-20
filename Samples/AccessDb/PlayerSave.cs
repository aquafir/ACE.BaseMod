using ACE.Database.Models.Shard;

namespace AccessDb;

public class PlayerSave
{
    //Todo: add some server identifier
    //public string Server { get; set; } = "";

    public Character Character { get; set; }
    public List<ACE.Database.Models.Shard.Biota> Inventory { get; set; }
    public List<ACE.Database.Models.Shard.Biota> Wielded { get; set; }
    public ACE.Database.Models.Shard.Biota PlayerBiota { get; set; }
}
