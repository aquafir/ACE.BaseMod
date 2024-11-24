using ACE.Common.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace Expansion;

/// <summary>
/// Patch settings are used to serialized patches
/// </summary>
public class MutatorSettings
{
    public const string NAMESPACE = $"Expansion";
    public string Mutation { get; set; }
    public bool Enabled { get; set; } = false;
    public MutationEvent Events { get; set; } = MutationEvent.Loot;
    public string? Odds { get; set; }
    public string? TreasureTargets { get; set; }
    public string? WeenieTypeTargets { get; set; }
    //public MutatorSettings() { }

    public MutatorSettings(string mutation, bool enabled = true)
    {
        Mutation = mutation;
        Enabled = enabled;
    }
}

public static class MutatorHelpers
{
    static readonly Dictionary<string, Type> mutators = new();

    /// <summary>
    /// Get a patch type and try to create an instance of the corresponding class
    /// </summary>
    public static Mutator GetMutator(this MutatorSettings settings)
    {
        //Get the type
        if (!mutators.TryGetValue(settings.Mutation, out var mutatorType))
        {
            mutatorType = CreateMutator(settings);
            mutators.TryAdd(settings.Mutation, mutatorType);
        }

        var patchInstance = Activator.CreateInstance(mutatorType);
        if (patchInstance is not Mutator mutator)
        {
            Debugger.Break();
            throw new Exception();
        }

        mutator.Event = settings.Events;

        //Nullable odds?
        mutator.TreasureTargets = S.Settings.TargetGroups.TryGetValue(settings.TreasureTargets ?? "", out var treasureTargets) ? treasureTargets.ToHashSet() : null;
        mutator.WeenieTypeTargets = S.Settings.WeenieTypeGroups.TryGetValue(settings.WeenieTypeTargets ?? "", out var weenieTargets) ? weenieTargets.ToHashSet() : null;
        mutator.Odds = S.Settings.Odds.TryGetValue(settings.Odds ?? "", out var mutatorOdds) ? mutatorOdds : null;

        ModManager.Log($"{mutator.TreasureTargets is null} - {mutator.WeenieTypeTargets is null} - {mutator.Odds is null}");

        return mutator;
    }

    private static Type CreateMutator(this MutatorSettings settings)
    {
        //Mutators match enum
        var type = Type.GetType($"{MutatorSettings.NAMESPACE}.{settings.Mutation}");

        if (type is null || !type.IsSubclassOf(typeof(Mutator)))
            return null;

        return type;
    }

    //public static void LoadMutators(string path)
    //{
    //    mutators.Clear();

    //    // Get the invoking assembly's namespace and references
    //    var ns = MutatorSettings.NAMESPACE;
    //    Assembly invokingAssembly = Mod.Instance.Container.ModAssembly; // Assembly.GetExecutingAssembly();
        
    //    List<MetadataReference> references = AppDomain.CurrentDomain
    //        .GetAssemblies()
    //        .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
    //        .Select(a => MetadataReference.CreateFromFile(a.Location))
    //        .Cast<MetadataReference>()
    //        .ToList();

    //    //Borrow referenced assemblies?
    //    //var usings = String.Join("\n", invokingAssembly.GetReferencedAssemblies().Where(x => !x.Name.StartsWith('0')).Select(x => $"using {x.Name};"));
    //    //var header = $$"""
    //    //    namespace Expansion;
    //    //    using HarmonyLib;
    //    //    {{usings}}

    //    //    """;

    //    var usingsPath = Path.Combine(path, "Usings.txt");
    //    List<UsingDirectiveSyntax> defaultUsings = new();
    //    if (File.Exists(usingsPath))
    //    {
    //        var usingsTree = CSharpSyntaxTree.ParseText(File.ReadAllText(usingsPath));
    //        defaultUsings = usingsTree.GetUsings().ToList();
    //    }

    //    //AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(invokingAssembly);

    //    //Try to load a Type 
    //    foreach (var mutator in Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories))
    //    {
    //        //Could wrap to let just the relevant code get used
    //        // Define the code to compile, including the target namespace
    //        var code = File.ReadAllText(mutator);
    //        if (code is null)
    //        {
    //            ModManager.Log($"Failed to read code from: {mutator}", ModManager.LogLevel.Error);
    //            continue;
    //        }

    //        // Parse the code into a syntax tree
    //        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            
    //        //Add default usings
    //        syntaxTree = syntaxTree.AddUsingsToSyntaxTree("DynamicTypeTest");

    //        //Get usings?
    //        Debugger.Break();            
    //        var usings = GetUsings(syntaxTree).Select(x => x.Name.ToString());
    //        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    //            .WithUsings(usings); // Automatically adds the specified using directives

    //        //Assume name is the file name, could try to do it with the syntax tree
    //        var name = Path.GetFileNameWithoutExtension(mutator);

    //        // Create the Roslyn compilation
    //        CSharpCompilation compilation = CSharpCompilation.Create(
    //            "DynamicAssembly",
    //            new[] { syntaxTree },
    //            references,
    //            options
    //        );

    //        // Compile the code into an in-memory assembly
    //        using var ms = new MemoryStream();
    //        EmitResult result = compilation.Emit(ms);
    //        Debugger.Break();

    //        if (!result.Success)
    //        {
    //            // Show compilation errors
    //            var errors = String.Join("\n", result.Diagnostics);
    //            ModManager.Log($"Failed to compile code from: {mutator}\n{errors}", ModManager.LogLevel.Error);
    //            continue;
    //        }

    //        // Load the compiled assembly
    //        ms.Seek(0, SeekOrigin.Begin);
    //        Assembly dynamicAssembly = Assembly.Load(ms.ToArray());
    //        //Assembly dynamicAssembly = context.LoadFromStream(ms);

    //        // Get the dynamically created type
    //        Type dynamicType = dynamicAssembly.GetType($"{ns}.{name}");

    //        if(dynamicType is null || !dynamicType.IsSubclassOf(typeof(Mutator))) {
    //            ModManager.Log($"Failed to load Mutator {mutator}", ModManager.LogLevel.Error);
    //            continue;
    //        }

    //        mutators.TryAdd(name, dynamicType);
    //    }
    //}

    //public static Assembly CreateDynamicAssemblyFromFiles(string[] filePaths)
    //{
    //    // Create an assembly and a module
    //    var assemblyName = new AssemblyName("DynamicAssembly");
    //    var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
    //    var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

    //    // Compile and add types for each file
    //    foreach (var filePath in filePaths)
    //    {
    //        string code = File.ReadAllText(filePath);
    //        CompileAndAddType(moduleBuilder, code);
    //    }

    //    return assemblyBuilder;
    //}

    //public static void CompileAndAddType(ModuleBuilder moduleBuilder, string code)
    //{
    //    // Parse and compile the code using Roslyn
    //    var syntaxTree = CSharpSyntaxTree.ParseText(code);
    //    var compilation = CSharpCompilation.Create("DynamicAssembly")
    //        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
    //        .AddSyntaxTrees(syntaxTree);

    //    using (var ms = new MemoryStream())
    //    {
    //        var result = compilation.Emit(ms);
    //        if (result.Success)
    //        {
    //            ms.Seek(0, SeekOrigin.Begin);
    //            var assembly = Assembly.Load(ms.ToArray());

    //            // Load types from the compiled assembly
    //            foreach (var type in assembly.GetTypes())
    //            {
    //                // Define type in the main assembly using TypeBuilder
    //                var typeBuilder = moduleBuilder.DefineType(type.Name, TypeAttributes.Public);
    //                foreach (var method in type.GetMethods())
    //                {
    //                    // Define methods and copy members from the compiled type
    //                    //DefineMethod(typeBuilder, method);
    //                }
    //                typeBuilder.CreateType();
    //            }
    //        }
    //        else
    //        {
    //            foreach (var diagnostic in result.Diagnostics)
    //            {
    //                Console.WriteLine(diagnostic.ToString());
    //            }
    //        }
    //    }
    //}

    //public static IEnumerable<UsingDirectiveSyntax> GetUsings(this SyntaxTree tree)
    //{
    //    if (tree is null)
    //        return default;
        
    //    return tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
    //}
}



// This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies
// of the plugin from the plugin's binary directory.
class SharedAssemblyLoadContext : AssemblyLoadContext
{
    // Resolver of the locations of the assemblies that are dependencies of the
    // main plugin assembly.
    private AssemblyDependencyResolver _resolver;

    public SharedAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
    // into the HostAssemblyLoadContext together with the plugin assembly itself.
    // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
    // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
    // The types present on the host and plugin side would then not match even though they would have the same names.
    protected override Assembly Load(AssemblyName name)
    {
        string assemblyPath = _resolver.ResolveAssemblyToPath(name);
        if (assemblyPath != null)
        {
            Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }
}



public static class CodeAnalysisHelpers
{
    public static Assembly LoadFromCode(this AssemblyLoadContext context, string code, string path)
    {
        Assembly invokingAssembly = Assembly.GetExecutingAssembly(); //Mod.Instance.Container.ModAssembly;

        List<MetadataReference> references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();
        //references.Add(MetadataReference.CreateFromFile(Mod.Instance.Container.DllPath));
        references.AddRange(GetReferencesFromPath(path));

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

        //var usings = GetUsings(syntaxTree).Select(x => x.Name.ToString());
        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        //.WithUsings(usings); // Automatically adds the specified using directives

        //Assume name is the file name, could try to do it with the syntax tree
        //var name = Path.GetFileNameWithoutExtension(mutator);

        // Create the Roslyn compilation
        CSharpCompilation compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            new[] { syntaxTree },
            references,
            options
        );

        //Compile the code
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        if (!result.Success)
        {
            var errors = String.Join("\n", result.Diagnostics);
            Console.WriteLine($"Failed to compile code from: \n{errors}");
            return null;
        }

        //Load the compiled assembly
        ms.Seek(0, SeekOrigin.Begin);
        return context.LoadFromStream(ms);
    }

    public static SyntaxTree AddUsingsToSyntaxTree(this SyntaxTree syntaxTree, params string[] usings)
    {
        var root = syntaxTree.GetRoot();

        // Create the using directives
        var usingDirectives = usings
            .Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)))
            .ToArray();

        // Add the using directives to the top of the syntax tree
        var newRoot = ((CompilationUnitSyntax)root).AddUsings(usingDirectives);
        return syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options);
    }

    public static IEnumerable<UsingDirectiveSyntax> GetUsings(this SyntaxTree tree)
    {
        if (tree is null)
            return default;

        return tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
    }

    public static IEnumerable<MetadataReference> GetReferencesFromPath(string path)
    {
        var references = new List<MetadataReference>();
        //MetadataReferenceResolver r = new();

        var dlls = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        foreach (var dll in dlls)
            references.Add(MetadataReference.CreateFromFile(dll));

        return references;
    }
}