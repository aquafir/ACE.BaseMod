using ACE.Database;
using ACE.Database.Adapter;
using ACE.DatLoader.FileTypes;
using ACE.Server;
using ACE.Server.Command.Handlers;
using ACE.Server.Command.Handlers.Processors;
using ACE.Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Shared.Helpers;
public static class ContentHelpers
{
    private static FileInfo[] GetCustomJsonFiles()
    {
        DirectoryInfo di = DeveloperContentCommands.VerifyContentFolder(null);
        if (!di.Exists) return [];

        return di.GetFiles("*.json", SearchOption.AllDirectories);
    }

    private static FileInfo[] GetCustomSqlFiles()
    {
        DirectoryInfo di = DeveloperContentCommands.VerifyContentFolder(null);
        if (!di.Exists) return [];

        return di.GetFiles("*.sql", SearchOption.AllDirectories);
    }

    private static List<uint> GetCustomWcids()
    {
        List<uint> wcids = new ();
        Regex re = new Regex(@"^\d+");

        var files = GetCustomJsonFiles().AddRangeToArray(GetCustomSqlFiles());
        foreach (var file in files)
        {
            var m = re.Match(file.Name);
            if (!m.Success)
                continue;

            if (uint.TryParse(m.Value, out var wcid))
                wcids.Add(wcid);
        }

        return wcids;
    }

    public static IEnumerable<Weenie> GetCustomWeenies()
    {
        Debugger.Break();
        var wcids = GetCustomWcids();
        foreach(var wcid in wcids)
        {
            var dbWeenie = DatabaseManager.World.GetWeenie(wcid);
            if (dbWeenie is null)
                continue;

            yield return WeenieConverter.ConvertToEntityWeenie(dbWeenie);
        }

        //Todo: decide if performance matters enough to add EF reference / make a local DbContext
        //DatabaseManager.World.ContextFactory.CreateDbContext()
        //var dbWeenies = wcids.Select(x => DatabaseManager.World.GetWeenie(x)).Where(w => w is not null);
        //var weenies = dbWeenies.Select(x => WeenieConverter.ConvertToEntityWeenie(x));        
        //return weenies.ToList();
    }
}
