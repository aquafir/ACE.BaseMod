using ACE.Database.Models.World;
using ACE.Entity.Enum.Properties;
using ACE.Server.Network.GameMessages.Messages;
using ACE.Server.Network;
using ImGuiNET;
using ACE.Entity;
using ACE.Database;
using System.Diagnostics;
using ACE.Server.Command.Handlers;
using ACE.Server.Physics.Common;

namespace ImGuiTest;

public class WorldTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    string query = "";
    bool focusQuery = true;
    public override void Render()
    {
        if(focusQuery)
        {
            ImGui.SetKeyboardFocusHere();
            focusQuery = false;
        }
        if (ImGui.InputText("Query", ref query, 100, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
        {
            QueryDb();
            focusQuery = true;
        }

        ImGui.SameLine();
        if (ImGui.Button("Search###WorldDb"))
        {
            QueryDb();
            focusQuery = true;
        }

            ImGui.Combo("Type ###WorldWeenieType", ref weenieTypeIndex, weenieTypes, weenieTypes.Length);

        ImGui.SameLine();
        if (ImGui.Button("Cache###CacheWorldDb"))
            CacheDb();


        if (ImGui.BeginTable("WorldTable", 3, TABLE_FLAGS))
        {
            // Set up columns
            int columnIndex = 0;
            ImGui.TableSetupColumn($"ID", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Name", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Class Name", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            //foreach (var p in players)
            foreach (var item in weenies)
            {
                //Check if skipped?
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text($"{item.WeenieClassId}");

                ImGui.TableNextColumn();
                if (!item.PropertiesString.TryGetValue(PropertyString.Name, out var name))
                    name = "n/a";
                ImGui.Text($"{name}");

                if (ImGui.BeginPopupContextItem($"Popup{item.WeenieClassId}"))
                {
                    if (ImGui.MenuItem("Create"))
                        AdminCommands.HandleCreate(GUI.Selected.Session, item.WeenieClassId.ToString());
                    if (ImGui.MenuItem("CreateInv"))
                        AdminCommands.HandleCI(GUI.Selected.Session, item.WeenieClassId.ToString());

                    ImGui.EndPopup();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"{item.ClassName}");
            }
            ImGui.EndTable();
        }
    }


    private void CacheDb()
    {
        var timer = Stopwatch.StartNew();
        DatabaseManager.World.CacheAllWeenies();
        var ms = timer.ElapsedMilliseconds;
        timer.Stop();

        GUI.Selected?.SendMessage($"Cached in {ms}ms");
        ModManager.Log($"Cached in {ms}ms");
    }


    int weenieTypeIndex = 10;
    string[] weenieTypes = Enum.GetNames<WeenieType>();

    List<ACE.Entity.Models.Weenie> weenies = new();

    public WorldTab(string label) : base(label)
    {
    }

    /// <summary>
    /// Displays creature types and the number of types of creatures belonging to that type
    /// </summary>
    private void QueryDb()
    {
        if (query is null)
            return;

        //Defaults to creature
        WeenieType type = (WeenieType)weenieTypeIndex;
        if (!DatabaseManager.World.weenieCacheByType.TryGetValue(type, out var creatureCache))
            return;

        weenies.Clear();
        weenies = creatureCache.Where(x => x.PropertiesString[PropertyString.Name].Contains(query, StringComparison.OrdinalIgnoreCase)).Take(20).ToList();

            //    using (var ctx = new WorldDbContext())
            //{
            //    // Group creatures by type
            //    var results = from creature in ctx.Weenie
            //                  where creature.Type == (int)(WeenieType.Creature) && creature.ClassName.Contains(query, StringComparison.OrdinalIgnoreCase)
            //                  join cType in ctx.WeeniePropertiesInt on creature.ClassId equals cType.ObjectId
            //                  where cType.Type == (ushort)(PropertyInt.CreatureType)
            //                  select new TableRow()
            //                  {
            //                      ID = (int)creature.ClassId,
            //                      Name = creature.ClassName,
            //                      Value = ((CreatureType)cType.Value).ToString(),
            //                  };
            //    tableData = results.ToArray();

            //var sb = new StringBuilder($"\n\n{"Name",-40}{"Type",-15}{"Type #",-10}\n");
            //foreach (var group in results.ToList().GroupBy(x => x.Type).OrderBy(x => x.Count()))
            //    sb.AppendLine($"{(CreatureType)group.Key,-40}{group.Key,-15}{group.Count(),-10}");

            //ModManager.Log(sb.ToString());
        
    }
}
