namespace ImGuiHud.Components.Pickers;
internal class PlayerPicker : IPagedPicker<Player>
{
    public override void Init()
    {
        Choices = PlayerManager.GetAllOnline();
        base.Init();
    }

    public override void DrawPageControls()
    {
        //base.DrawPageControls();
    }

    public override void DrawItem(Player item, int index)
    {
        if(ImGui.Button($"{item?.Name}"))
            SelectItem(item, index);
    }
}


