//public class EquipmentPicker : TexturedPicker<EquipmentSlot>
//{
//    //Order things are drawn in determined by the DefaultIcons order
//    private static readonly EquipmentSlot[] Slots = EquipmentHelper.DefaultIcons.Keys.ToArray(); //Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToArray();    

//    public EquipmentPicker() : base(GetEquipmentSlotId, Slots) { PerPage = 30; }

//    /// <summary>
//    /// Gets the default icon or the icon of the equipment in the given slot
//    /// </summary>
//    private static ManagedTexture GetEquipmentSlotId(EquipmentSlot x) =>
//        x.TryGetSlotEquipment(out var wo) ?
//        wo.GetOrCreateTexture() :
//        TextureManager.GetOrCreateTexture(EquipmentHelper.DefaultIcons[x]);

//    //Manage linebreaks?
//    public override void DrawItem(EquipmentSlot item, int index)
//    {
//        if (index != 5 && index != 10 && index != 15 && index != 19)
//            ImGui.SameLine();
//        else
//            ImGui.NewLine();

//        base.DrawItem(item, index);

//        if (ImGui.IsItemHovered())
//        {
//            if (item.TryGetSlotEquipment(out var wo))
//            {
//                wo.DrawTooltip();
//                //var desc = wo.Describe();

//                //ImGui.SetNextWindowSizeConstraints(new(300), new(500));
//                //ImGui.BeginTooltip();
//                ////Icon
//                //var texture = wo.GetOrCreateTexture();
//                //ImGui.TextureButton($"{wo.Id}", texture, IconSize);
//                //ImGui.SameLine();

//                ////ImGui.TextWrapped(desc);
//                //ImGui.EndTooltip();
//            }
//        }
//    }

//    //No paging and enough items drawn to show all on one page
//    public override void DrawPageControls() { }
//}
