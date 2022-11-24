using ACE.Database.Models.Shard;

namespace PlayerSave;

public class PlayerSave
{
    //Todo: add some server identifier
    //public string Server { get; set; } = "";

    public Character Character { get; set; }
    public List<Biota> Inventory { get; set; }
    public List<Biota> Wielded { get; set; }
    public Biota PlayerBiota { get; set; }
}
