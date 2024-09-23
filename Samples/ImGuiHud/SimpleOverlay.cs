using ClickableTransparentOverlay;

namespace ImGuiHud;

public class SimpleOverlay : Overlay
{
    bool isRunning = true;

    FlagsPicker<SpellFlags> Flags = new();
    TextSpellPicker SpellPicker = new();

    public SimpleOverlay() : base(1920, 1080) { }

    protected override Task PostInitialized()
    {
        VSync = false;
        return Task.CompletedTask;
    }

    protected override void Render()
    {
        bool isCollapsed = !ImGui.Begin(
            "Overlay Main Menu",
            ref isRunning,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

        if (!isRunning || isCollapsed)
        {
            ImGui.End();
            if (!isRunning)
                Close();

            return;
        }

        ImGui.Text("Hello!");

        if (Flags.Check())
        {
            ModManager.Log($"Selected {Flags.Selection}!");
        }

        if (SpellPicker.Check())
            ModManager.Log($"Selected {SpellPicker.Selection.Name}!");

        ImGui.End();
    }
}
