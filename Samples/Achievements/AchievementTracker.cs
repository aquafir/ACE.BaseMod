using ACE.Server.Managers;
using Tinkering.Data;
using Tinkering.Domain;

namespace Tinkering;

public class AdventurerTracker
{
    List<Adventurer> _adventurers = new();
    AchievementDbContext _context;


    public AdventurerTracker()
    {
        _context = new();
    }
    public AdventurerTracker(AchievementDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        foreach (var player in PlayerManager.GetAllOnline())
            TrackOrAddAdventurer(player);
    }

    public void TrackOrAddAdventurer(Player player)
    {
        var adventurer = _context.Adventurers.Find(player.Character.Id);

        if (adventurer is null)
        {
            adventurer = new Adventurer
            {
                AdventurerId = player.Character.Id,
                Kills = new(),
                Lands = new(),
            };

            _context.ChangeTracker.DetectChanges();
            Debugger.Break();
            ModManager.Log($"Tracking achievements of {player.Name} - {player.Character.Id}");
        }
    }

    public void Shutdown()
    {
        _context.SaveChanges();
    }

}
