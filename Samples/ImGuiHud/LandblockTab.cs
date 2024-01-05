using ImGuiNET;

namespace ImGuiTest;

public class LandblockTab : Widget
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.Sortable |
            ImGuiTableFlags.RowBg |
            ImGuiTableFlags.ScrollY |
            ImGuiTableFlags.BordersOuter |
            ImGuiTableFlags.BordersV |
            ImGuiTableFlags.ContextMenuInBody;

    public LandblockTab(string label) : base(label)
    {
    }

    public override void Render()
    {
        if (GUI.Selected is null)
        {
            ImGui.Text("Select a player.");
            return;
        }

        var lb = GUI.Selected.CurrentLandblock;
        if (lb is null)
            return;

        if (ImGui.CollapsingHeader($"LBPlayers"))
        {
            if (ImGui.BeginTable("LBPlayerTable", 3, TABLE_FLAGS))
            {
                // Set up columns
                int columnIndex = 0;
                ImGui.TableSetupColumn($"ID", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
                ImGui.TableSetupColumn($"Name", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
                ImGui.TableSetupColumn($"{lb.Id.Raw}", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

                // Headers row
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                foreach (var player in lb.players)
                {
                    //Check if skipped?
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    ImGui.Text($"{player.Character.Id}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"Selected: {player.Name}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{player.Level.ToString()}");
                }
                ImGui.EndTable();
            }
        }

        if (ImGui.CollapsingHeader($"LBGen"))
        {
            //Each generator has a collapsing section
            foreach (var gen in lb.sortedGeneratorsByNextRegeneration)
            {
                if (ImGui.CollapsingHeader($"{gen.CurrentCreate}/{gen.MaxCreate}###H{gen.Guid.Full}"))
                {
                    if (ImGui.BeginPopupContextItem($"GenPopup{gen.Guid.Full}"))
                    {
                        if (ImGui.MenuItem($"Respawn###{gen.Guid.Full}"))
                        {
                            foreach (var profile in gen.GeneratorProfiles)
                            {
                                //profile.SpawnQueue.Add(DateTime.MinValue);
                                var num = profile.MaxCreate - profile.CurrentCreate;
                                GUI.Selected.SendMessage($"{profile.Id} wants to respawn {num}");
                                profile.Enqueue(num);
                                profile.ProcessQueue();
                            }
                        }
                        if (ImGui.MenuItem($"Smite###{gen.Guid.Full}"))
                        {
                            foreach (var profile in gen.GeneratorProfiles)
                            {
                                //Can crash physics
                                //profile.DestroyAll();
                                foreach (var spawn in profile.Spawned)
                                {
                                    spawn.Value?.TryGetWorldObject()?.Destroy();
                                }
                            }
                        }


                        ImGui.EndPopup();
                    }


                    if (ImGui.BeginTable($"GenTable{gen.Guid.Full}", 3, TABLE_FLAGS))
                    {
                        // Set up columns
                        int columnIndex = 0;
                        ImGui.TableSetupColumn($"Spawned", ImGuiTableColumnFlags.DefaultSort, 0, (uint)columnIndex++);
                        ImGui.TableSetupColumn($"Type", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);
                        ImGui.TableSetupColumn($"Next (min)", ImGuiTableColumnFlags.PreferSortAscending, 0, (uint)columnIndex++);

                        // Headers row
                        ImGui.TableSetupScrollFreeze(0, 1);
                        ImGui.TableHeadersRow();

                        foreach (var profile in gen.GeneratorProfiles)
                        {
                            //Check if skipped?
                            ImGui.TableNextRow();
                            ImGui.TableNextColumn();
                            ImGui.Text($"{profile.Spawned.Count}/{profile.MaxCreate}");

                            ImGui.TableNextColumn();
                            ImGui.Text($"Selected: {profile.RegenLocationType}");

                            if (ImGui.BeginPopupContextItem($"Popup{profile.Id}"))
                            {
                                if (ImGui.MenuItem($"Spawn###{profile.Id}"))
                                    profile.Spawn();

                                ImGui.EndPopup();
                            }

                            ImGui.TableNextColumn();
                            ImGui.Text($"{(profile.NextAvailable - DateTime.Now).TotalMinutes}");
                        }

                        ImGui.EndTable();
                    }
                }
            }
        }
    }
}