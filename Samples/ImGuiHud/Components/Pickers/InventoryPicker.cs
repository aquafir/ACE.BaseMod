//public class InventoryPicker : TexturedPicker<WorldObject>
//{
//  //  RegexFilter<WorldObject> nameFilter = new(x => x.Name) { Active = false, Label = "Name" };
//    //PropFilter
////    FilterSet<WorldObject> filter;

//    public InventoryPicker() : base(x => x.GetOrCreateTexture(), new WorldObject[] {})
//    {
//        //Assume 1k as a max inventory
//        PerPage = 1000;
//        //filter = new(new() {
//        //    nameFilter
//        //});

//        Mode = SelectionStyle.Multiple;

//        //Update();
//    }

//    public override void DrawBody()
//    {
//        //if (filter.Check())
//            //Update();
//    //    if (nameFilter.Check())
//      //      Update();
//        base.DrawBody();
//    }

//    public override void DrawItem(WorldObject item, int index)
//    {
//        base.DrawItem(item, index);

//        item.AddDragDrop();

//        if (ImGui.IsItemHovered() && !ImGui.IsMouseDragging(ImGuiMouseButton.Left))
//            item.DrawTooltip();
//    }

//    public void Update()
//    {
//        //Choices = G.Game.Character.Inventory.ToArray();
//            //.Where(x => !nameFilter.IsFiltered(x)).ToArray();
//    }

//    //Don't show controls
//    public override void DrawPageControls() { }
//}

//public class ContainerPicker : TexturedPicker<WorldObject>
//{
//    public ContainerPicker() : base(x => x.GetOrCreateTexture(), GetPlayerContainers()) { }

//    public static WorldObject[] GetPlayerContainers() => G.Game.Character.Containers.Prepend(G.Game.Character.Weenie).ToArray();

//    public override void DrawItem(WorldObject item, int index)
//    {
//        var selected = Selection is not null && Selection.Equals(item);

//        Vector4 bg = selected ? new(.6f) : new(0);

//        var icon = textureMap(item);
//        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize,0,bg))
//            SelectItem(item, index);

//        //Draw capacity
//        if (item.TryGet(IntId.ItemsCapacity, out var cap))
//        {
//            var size = ImGui.GetItemRectSize();
//            var start = ImGui.GetItemRectMin();
//            start.X += size.X - 3;
//            start.Y += size.Y * (1 - (float)item.Items.Count / cap);
//            var end = ImGui.GetItemRectMax();
//            var dl = ImGui.GetWindowDrawList();
//            dl.AddRectFilled(start, end, 0xFF00FFFF);

//        }

//        HandleDrag(item);


//        if (ImGui.IsItemHovered())
//            item.DrawTooltip();
//    }

//    private void HandleDrag(WorldObject item)
//    {
//        item.AddDragDrop();

//        if(item.TryAcceptDrop(out var payload))
//        {
//            ModManager.Log($"Drop {payload.Name}->{item.Name}");
//            //Dropping a container?
//            if (payload.TryGet(IntId.ItemsCapacity, out var capacity))
//            {
//                //Reorder?
//                var parent = item.Container ?? item;
//                var index = item.Container is null ? 0 : item.Container.Containers.IndexOf(item);
//                payload.Move(parent.Id, (uint)index);
//                ModManager.Log($"Move to index {index}");
//            }
//            //Move item?
//            else if (payload.ContainerId == G.Game.CharacterId)
//            {
//                payload.Move(item.Id);
//            }

//            Changed = true;
//        }
        

//    }

//    //Don't show controls
//    public override void DrawPageControls() { }
//}

