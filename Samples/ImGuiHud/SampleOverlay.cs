using ClickableTransparentOverlay;
using ClickableTransparentOverlay.Win32;

namespace ImGuiHud;

public class SampleOverlay : Overlay
{
    private volatile State state;
    private readonly Thread logicThread;

    public SampleOverlay() : base(1920, 1080)
    {
        //Based on SampleOverlay, which has a background logic thread example if needed
        //https://github.com/zaafar/ClickableTransparentOverlay/blob/master/Examples/MultiThreadedOverlay/SampleOverlay.cs
        state = new State();
    }

    protected override void Render()
    {
        //ImGui.SetNextWindowPos(new Vector2(0f, 0f));
        ImGui.SetNextWindowSizeConstraints(new(300), new(1000));
        //ImGui.SetNextWindowBgAlpha(0.9f);
        ImGui.Begin("Overly", ImGuiWindowFlags.AlwaysAutoResize);

        RenderMainMenu();
        ImGui.Separator();

        for (var i = 0; i < 10; i++)
            ImGui.Text($"Item {i}");

        ImGui.End();

        return;
    }

    private void RenderMainMenu()
    {

        if (state.Player is null && ImGui.Button("Select Player"))
            state.Player = PlayerManager.GetAllOnline().FirstOrDefault();
        else if (ImGui.Button($"{state.Player?.Name}"))
            state.Player = null;
    }
}


public class State
{
    public bool IsRunning = true;
    public bool Visible;
    public Player Player;
    public WorldObject Selected;
}
