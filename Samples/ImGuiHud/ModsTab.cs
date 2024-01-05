using ImGuiNET;

namespace ImGuiTest;

public class ModsTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    public ModsTab(string label) : base(label)
    {
    }

    public override void Render()
    {
        if (ImGui.Button("Close"))
        {
            PatchClass.Overlay.Close();
        }

        if (ImGui.BeginTable("ModsTable", 3, TABLE_FLAGS))
        {
            // Set up columns
            int columnIndex = 0;
            ImGui.TableSetupColumn($"Name", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
            ImGui.TableSetupColumn($"Status", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

            // Headers row
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableHeadersRow();

            foreach (var mod in ModManager.Mods)
            {
                //Check if skipped?
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text($"{mod.TypeName}");

                ImGui.TableNextColumn();
                ImGui.Text($"Selected: {mod.Status}");

                if (ImGui.BeginPopupContextItem($"Mod{mod.TypeName}"))
                {
                    if (ImGui.MenuItem("Restart"))
                        mod.Restart();
                    if (ImGui.MenuItem("Toggle"))
                    {
                        if (mod.Status == ModStatus.Active)
                            ModManager.EnableModByName(mod.TypeName);
                        else
                            ModManager.DisableModByName(mod.TypeName);
                    }
                }
                ImGui.EndPopup();
            }
        }
        ImGui.EndTable();
    }
}
