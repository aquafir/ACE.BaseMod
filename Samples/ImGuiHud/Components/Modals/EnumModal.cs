
public class EnumModal(Type type) : IModal
{
    public Vector2 IconSize = new(24);

    protected EnumPicker picker = new(type);

    public override void DrawBody()
    {        
        if (picker.Check())
        {
            if (picker.Changed)
            {
                ModManager.Log($"{picker.Selection}");
                picker.Changed = false;
            }
        }
    }
}
