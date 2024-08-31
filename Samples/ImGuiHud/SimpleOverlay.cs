using ClickableTransparentOverlay;

namespace ImGuiHud;

public class SimpleOverlay : Overlay
{
    private bool wantKeepDemoWindow = true;

    public SimpleOverlay() : base(1920, 1080)
    {
    }

    protected override Task PostInitialized()
    {
        this.VSync = false;
        return Task.CompletedTask;
    }

    protected override void Render()
    {
        RenderMainMenu();
        //ImGui.ShowDemoWindow(ref wantKeepDemoWindow);
        //if (!this.wantKeepDemoWindow)
        //{
        //    this.Close();
        //}
    }

    bool isRunning = true;
    private void RenderMainMenu()
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

        ImGui.End();
    }
}
