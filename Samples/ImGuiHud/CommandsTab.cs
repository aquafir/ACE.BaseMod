using ImGuiNET;

namespace ImGuiTest;

public class CommandsTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    public CommandsTab(string label) : base(label)
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
            ImGui.TableSetupColumn($"Command", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Usage", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Description", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            foreach (var command in CommandManager.commandHandlers)
            {
                var attr = command.Value.Attribute;

                //Check if skipped?
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                if (ImGui.Button($"{attr.Command}"))
                {
                    GUI.Selected.SendMessage($"Selected {attr.Command}");
                }


                ImGui.TableNextColumn();
                ImGui.Text($"Selected: {attr.Usage}");

                //if (ImGui.BeginPopupContextItem($"Popup{item.Guid.Full}"))
                //{
                //    if (ImGui.MenuItem("Drop"))
                //    {
                //        GUI.Selected.SendMessage($"Dropping {item.Name}");
                //        GUI.Selected.HandleActionDropItem(item.Guid.Full);
                //    }
                //    if (ImGui.MenuItem("Destroy"))
                //    {
                //        GUI.Selected.SendMessage($"Destroying {item.Name}");
                //        item.Destroy();
                //    }

                //   ImGui.EndPopup();
                //}

                ImGui.TableNextColumn();
                ImGui.Text($"{attr.Description}");
            }

            ImGui.EndTable();
        }
    }
}