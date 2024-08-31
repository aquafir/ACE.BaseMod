

public class FlagsModal(Type type) : IModal
{
    public Vector2 IconSize = new(24);

    public FlagsPicker Picker = new(type);

    public override void DrawBody()
    {
        if(Picker.Check())
            ModManager.Log($"{Picker.Selection}");
    }

    public override void DrawFooter()
    {
        if (ImGui.Button("Save"))
            Save();

        ImGui.SameLine();
        if (ImGui.Button("Cancel"))
            Close();

        ImGui.SameLine();
        if (ImGui.Button("Clear"))
            Picker.Selection = 0;
    }
}
