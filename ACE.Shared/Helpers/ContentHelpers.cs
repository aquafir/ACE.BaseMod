using ACE.Adapter.Lifestoned;
using ACE.Database;
using ACE.Database.Adapter;

namespace ACE.Shared.Helpers;
public static class ContentHelpers
{
    private static DirectoryInfo VerifyContentFolder(string path = null)
    {
        path = path ?? PropertyManager.GetString("content_folder").Item;
        var sep = Path.DirectorySeparatorChar;

        // handle relative path
        if (path.StartsWith("."))
        {
            var cwd = Directory.GetCurrentDirectory() + sep;
            path = cwd + path;
        }

        var di = new DirectoryInfo(path);

        if (!di.Exists)
            throw new Exception($"Couldn't find content folder: {di.FullName}");

        return di;
    }

    private static FileInfo[] GetCustomJsonFiles(string path = null)
    {
        DirectoryInfo di = VerifyContentFolder(path);
        if (!di.Exists) return [];

        return di.GetFiles("*.json", SearchOption.AllDirectories);
    }

    private static FileInfo[] GetCustomSqlFiles(string path = null)
    {
        DirectoryInfo di = VerifyContentFolder(path);
        if (!di.Exists) return [];

        return di.GetFiles("*.sql", SearchOption.AllDirectories);
    }

    private static List<uint> GetCustomWcids(string path = null)
    {
        List<uint> wcids = new();
        Regex re = new Regex(@"^\d+");

        var files = GetCustomJsonFiles(path).AddRangeToArray(GetCustomSqlFiles(path));
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

    public static IEnumerable<Weenie> GetCustomWeenies(string path = null)
    {
        var wcids = GetCustomWcids(path);
        foreach (var wcid in wcids)
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















    public static bool TryLoadTemplate(string templatePath, out ACE.Entity.Models.Weenie template)
    {
        template = null;

        //Requires something different based on content type
        if (!LifestonedLoader.TryLoadWeenie(templatePath, out var lsWeenie))
        {
            Console.WriteLine("Error loading template from: " + templatePath);
            return false;
        }
        if (!LifestonedConverter.TryConvert(lsWeenie, out var dbWeenie))
        {
            Console.WriteLine("Error converting template");
            return false;
        }

        template = WeenieConverter.ConvertToEntityWeenie(dbWeenie);

        //Load a recipe
        // if(!LifestonedLoader.TryLoadRecipe(template, out var lsRecipeWeenie)) {
        //     Console.WriteLine("Error loading recipe template from: " + template);
        //     return;
        // }
        // if(!LifestonedConverter.TryConvert(lsRecipeWeenie, out var dbRecipeWeenie)) {
        //     Console.WriteLine("Error converting template");4
        //     return;
        // }
        // var recipeWeenie = WeenieConverter.ConvertToEntityWeenie(dbRecipeWeenie);

        return true;
    }
    /// <summary>
    /// Saves a ACE.Database.Models.World.Weenie
    /// </summary>
    //public static void SaveWeenie(string path, ACE.Entity.Models.Weenie weenie)
    //{

    //    var dbWeenie =  Helpers.ConvertFromEntityWeenie(weenie);

    //    if (!LifestonedConverter.TryConvertACEWeenieToLSDJSON(dbWeenie, out var json, out var json_weenie))
    //    {
    //        Console.WriteLine($"Failed to convert {dbWeenie.ClassId} - {dbWeenie.ClassName} to json");
    //        return;
    //    }

    //    File.WriteAllText(path, json);
    //}

    //public static Weenie ImportWeenie(string path)
    //{

    //}
}
