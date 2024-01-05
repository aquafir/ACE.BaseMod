using ACE.Entity.Enum.Properties;
using ACE.Server.Managers;
using ImGuiNET;
using System.Text.RegularExpressions;

namespace ImGuiTest;

public class PlayerTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    public override void Render()
    {
        RenderButtons();

        if (ImGui.BeginTable("PlayerTable", 3, TABLE_FLAGS))
        {
            // Set up columns
            int columnIndex = 0;
            ImGui.TableSetupColumn($"ID", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Name", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"{(keys.Length > 0 ? keys[keyIndex] : "n/a")}", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            //Sort if needed
            //Sort();

            //foreach (var p in players)
            foreach (var p in PlayerManager.GetAllOnline())
            {
                string propVal = "n/a";
                if (props.Length > 0 && keys.Length > 0)
                {
                    propVal = propType switch
                    {
                        PropertyType.PropertyBool => p.GetProperty((PropertyBool)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyDataId => p.GetProperty((PropertyDataId)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyDouble => p.GetProperty((PropertyFloat)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyInstanceId => p.GetProperty((PropertyInstanceId)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyInt => p.GetProperty((PropertyInt)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyInt64 => p.GetProperty((PropertyInt64)keyIndex)?.ToString() ?? "null",
                        PropertyType.PropertyString => p.GetProperty((PropertyString)keyIndex)?.ToString() ?? "null",
                        _ => "Unknown",
                    };
                }

                //Check if skipped?
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text($"{p.Character.Id}");

                ImGui.TableNextColumn();
                ImGui.Text($"Selected: {p.Character.Name}");

                if (ImGui.BeginPopupContextItem($"{p.Character.Id}"))
                {
                    if (ImGui.MenuItem("Select"))
                    {
                        GUI.Selected = p;
                        p.SendMessage("THE GUI GAZES AT YOU");
                    }

                    if (ImGui.MenuItem("Kick"))
                        p.SendMessage("Kick");

                    if (ImGui.MenuItem("Ban"))
                        p.SendMessage("Ban");

                    ImGui.EndPopup();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"{propVal}");
            }
            ImGui.EndTable();
        }
    }


    PropertyType propType = PropertyType.PropertyBook;
    int propIndex = 0;
    string[] props = Enum.GetNames<PropertyType>();

    int keyIndex = 0;
    string[] keys = new string[0];

    string query = "";
    List<Player> players = new();

    public PlayerTab(string label) : base(label)
    {
    }

    private void RenderButtons()
    {
        ImGui.InputText("Search###PlayerName", ref query, 100);
        ImGui.SameLine();
        if (ImGui.Button("Find Players"))
        {
            Regex regex = new(query);
            players.Clear();

            players = PlayerManager.GetAllOnline().Where(x => regex.IsMatch(x.Name)).ToList();
        }

        ImGui.PushItemWidth(200);
        if (ImGui.Combo("Prop###PropCombo", ref propIndex, props, props.Length))
        {
            propType = (PropertyType)propIndex;

            keys = propType switch
            {
                //PropertyType.PropertyAttribute => Enum.GetNames<>(),
                //PropertyType.PropertyAttribute2nd => Enum.GetNames<>(),
                //PropertyType.PropertyBook => Enum.GetNames<>(),
                PropertyType.PropertyBool => Enum.GetNames<PropertyBool>(),
                PropertyType.PropertyDataId => Enum.GetNames<PropertyDataId>(),
                PropertyType.PropertyDouble => Enum.GetNames<PropertyFloat>(),
                PropertyType.PropertyInstanceId => Enum.GetNames<PropertyInstanceId>(),
                PropertyType.PropertyInt => Enum.GetNames<PropertyInt>(),
                PropertyType.PropertyInt64 => Enum.GetNames<PropertyInt64>(),
                PropertyType.PropertyString => Enum.GetNames<PropertyString>(),
                //PropertyType.PropertyPosition => Enum.GetNames<>(),
                _ => new string[0],
            };
        }
        //ImGui.PushItemWidth(100);
        ImGui.SameLine();
        if (ImGui.Combo("Key###KeyCombo", ref keyIndex, keys, keys.Length))
        {
            //Todo
        }
        ImGui.PopItemWidth();
    }
}