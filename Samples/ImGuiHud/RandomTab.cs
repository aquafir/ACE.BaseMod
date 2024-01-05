using ACE.Database;
using ACE.Entity.Models;
using ACE.Server.Managers;
using ImGuiNET;
using System.Text.RegularExpressions;

namespace ImGuiTest;

//Possible reference: https://github.com/goatcorp/Dalamud/blob/5a5cc5701ab9dbf268b57e31b857bbe7a2513695/Dalamud/Interface/Utility/Table/Table.cs

public partial class RandomTab : Widget
{
    //Data for the table
    public TableRow[] tableData = new TableRow[]
    {
        new TableRow { ID = 1, Name = "John", Value = "42.5f" },
        new TableRow { ID = 2, Name = "Alice", Value = "18.3f" },
        new TableRow { ID = 3, Name = "Bob", Value = "33.7f" },
    };

    //Sort table based on column/direction
    private uint sortColumn = 0; // Currently sorted column index
    private ImGuiSortDirection sortDirection = ImGuiSortDirection.Ascending;

    private int CompareTableRows(TableRow a, TableRow b) => sortColumn switch
    {
        0 => a.ID.CompareTo(b.ID) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
        1 => a.Name.CompareTo(b.Name) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
        2 => a.Value.CompareTo(b.Value) * (sortDirection == ImGuiSortDirection.Descending ? -1 : 1),
    };

    //Sort if needed
    private void Sort()
    {
        var tableSortSpecs = ImGui.TableGetSortSpecs();

        //Check if a sort is needed
        if (tableSortSpecs.SpecsDirty)
        {
            //Set column/direction
            sortDirection = tableSortSpecs.Specs.SortDirection;
            sortColumn = tableSortSpecs.Specs.ColumnUserID;
            Console.WriteLine($"Dirty: {sortDirection} - {tableSortSpecs.Specs.ColumnUserID}");


            tableSortSpecs.SpecsDirty = false;
            Array.Sort(tableData, CompareTableRows);
        }
    }

    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;



    public override void Render()
    {
        RenderMOTD();



        ImGui.SameLine();
        if (ImGui.Button("Smite"))
        {
            GUI.Selected?.Destroy();
        }

        if (GUI.Selected is null)
            return;

        //if(ImGui.BeginTabBar("TabBar"))
        //{
        //    for(var i = 0; i < 3; i++)
        //    {
        //        if(ImGui.BeginTabItem($"Tab {i}"))
        //        {
        //            ImGui.Text($"Testing {i}");

        //            ImGui.EndTabItem();
        //        }
        //    }

        //    ImGui.EndTabBar();
        //}

        //ImGui.Text($"Hello, {PatchClass.Selected.Name}");
        //return;


        //  return;

        if (ImGui.BeginTable("MyTable", 3, TABLE_FLAGS))
        {
            // Set up columns
            int columnIndex = 0;
            ImGui.TableSetupColumn($"ID {columnIndex}", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Prop {columnIndex}", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Value {columnIndex}", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            //ImGui::PushItemWidth(-ImGui::GetContentRegionAvail().x * 0.5f);


            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            //Sort if needed
            Sort();

            for (int i = 0; i < tableData.Length; i++)
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].ID}");

                if (ImGui.BeginPopupContextItem())
                {
                    if (ImGui.MenuItem("Test123"))
                        Console.WriteLine("Clicked");
                    ImGui.EndPopup();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].Name}");

                ImGui.TableNextColumn();
                ImGui.Text($"{tableData[i].Value}");
            }

            ImGui.EndTable();
        }
    }

    private double dubs = 10;
    private string motd = "testing";
    private string input = "foo";



    private string[] weenieTypes = Enum.GetNames<WeenieType>();
    private int weenieType = 0;

    public RandomTab(string label) : base(label)
    {
    }

    private void RenderMOTD()
    {
        ImGui.InputText("Name", ref input, 100);

        ImGui.Combo("WeenieType", ref weenieType, weenieTypes, weenieTypes.Length);

        //WorldObject wo;
        //wo.ItemType == ItemType.

        //ImGui.InputTextMultiline("Server MOTD", ref motd, 1000, new System.Numerics.Vector2(400, 100));

        if (ImGui.Button("Search"))
        {
            //Weenie Type should correspond to class ID?
            Regex pattern = new Regex(input, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //var weenies = DatabaseManager.World.GetAllWeenies().Where(x => x.ClassId == weenieType && pattern.IsMatch(x.ClassName)).Take(10);
            var weenies = DatabaseManager.World.GetRandomWeeniesOfType(weenieType, 20);
            //Debugger.Break();
            tableData = weenies.Select(x => new TableRow()
            {
                ID = (int)x.WeenieClassId,
                Name = x.GetName(),
                Value = x.ClassName,
            }).ToArray();

            PropertyManager.ModifyString("server_motd", motd);
        }
    }
}