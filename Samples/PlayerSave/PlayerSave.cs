using Biota = ACE.Database.Models.Shard.Biota;

namespace PlayerSave;

public class PlayerSave
{
    //Todo: add some server identifier
    //public string Server { get; set; } = "";

    public Character Character { get; set; } = new();
    public List<Biota> Inventory { get; set; } = new();
    public List<Biota> Wielded { get; set; } = new();
    public Biota PlayerBiota { get; set; } = new();
}
