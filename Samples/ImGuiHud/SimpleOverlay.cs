using ClickableTransparentOverlay;

namespace ImGuiHud;

public class SimpleOverlay : Overlay
{
    bool isRunning = true;

    Player p;
    WorldObject t;

    FlagsPicker<SpellFlags> Flags = new();
    TextSpellPicker SpellPicker;

    public SimpleOverlay() : base(1920, 1080) {
        Flags = new();
        SpellPicker = new();
    }

    public override Task Run()
    {
        return base.Run();
    }

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
        DrawBody();

        ImGui.End();
    }

    private void DrawBody()
    {
        if (p is null && ImGui.Button("Select Player"))
            p = PlayerManager.GetAllOnline().FirstOrDefault();
        else if (ImGui.Button($"{p?.Name}"))
            p = null;

        if (p is null || p?.AttackTarget is not Creature c)
            return;

        ImGui.Text($"Selected: {c.Name}");

        //if (Flags.Check())
        //    ModManager.Log($"Selected {Flags.Selection}!");



        //if (SpellPicker.Check())
        //    ModManager.Log($"Selected {SpellPicker.Selection.Name}!");
    }
}
