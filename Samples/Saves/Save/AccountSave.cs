namespace Saves.Save;

public class AccountSave
{
    public string Name { get; set; }
    public List<PlayerSave> Players = new();
}
