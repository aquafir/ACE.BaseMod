namespace Discord;


public class FakePlayer
{
    // 0x60000000 -- 0x7 No PCAP'd GUID's in this range if that's relevant    
    public string Name { get; set; }
    public ObjectGuid Guid { get; set; }

    public FakePlayer(string name)
    {
        Name = name;
        Guid = GuidManager.NewPlayerGuid();     //Using a Player GUID makes it clickable to "/tell <name>,"
    }
}
