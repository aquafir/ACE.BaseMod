using ImGuiNET;

namespace ImGuiTest;

public class InventoryTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    public InventoryTab(string label) : base(label)
    {
    }

    public override void Render()
    {
        if (GUI.Selected is null)
        {
            ImGui.Text("Select a player.");
            return;
        }

        if (ImGui.BeginTable("PlayerTable", 3, TABLE_FLAGS))
        {
            // Set up columns
            int columnIndex = 0;
            ImGui.TableSetupColumn($"ID", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Name", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"n/a", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            //foreach (var p in players)
            foreach (var item in GUI.Selected.Inventory.Values)
            {
                //Check if skipped?
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text($"{item.Guid.Full}");

                ImGui.TableNextColumn();
                ImGui.Text($"Selected: {item.Name}");

                if (ImGui.BeginPopupContextItem($"Popup{item.Guid.Full}"))
                {
                    if (ImGui.MenuItem("Drop"))
                    {
                        GUI.Selected.SendMessage($"Dropping {item.Name}");
                        GUI.Selected.HandleActionDropItem(item.Guid.Full);
                    }
                    if (ImGui.MenuItem("Destroy"))
                    {
                        GUI.Selected.SendMessage($"Destroying {item.Name}");
                        item.Destroy();
                    }

                    ImGui.EndPopup();
                }

                ImGui.TableNextColumn();
                ImGui.Text($"{item.Value}");
            }
            ImGui.EndTable();
        }
    }
}