

//public class TextureGroupFilter : IOptionalFilter<TextureGroup>
//{
//    const int MIN_HEIGHT = 1;
//    const int MIN_WIDTH = 1;
//    const int MAX_HEIGHT = 2185;
//    const int MAX_WIDTH = 4096;

//    public int MinWidth = MIN_WIDTH;
//    public int MaxWidth = MAX_WIDTH;
//    public int MinHeight = MIN_HEIGHT;
//    public int MaxHeight = MAX_HEIGHT;

//    RegexFilter<TextureGroup> filter = new(x => x.ToString());

//    public override void DrawBody()
//    {
//        ImGui.NewLine();

//        ImGui.SetNextItemWidth(120);
//        if (ImGui.SliderInt($"MinX##{_id}", ref MinWidth, MIN_WIDTH, MAX_WIDTH))
//            Changed = true;
//        ImGui.SameLine();
//        ImGui.SetNextItemWidth(120);
//        if (ImGui.SliderInt($"MaxX##{_id}", ref MaxWidth, MIN_WIDTH, MAX_WIDTH))
//            Changed = true;

//        ImGui.SetNextItemWidth(120);
//        if (ImGui.SliderInt($"MinY##{_id}", ref MinHeight, MIN_HEIGHT, MAX_HEIGHT))
//            Changed = true;
//        ImGui.SameLine();
//        ImGui.SetNextItemWidth(120);
//        if (ImGui.SliderInt($"MaxY##{_id}", ref MaxHeight, MIN_HEIGHT, MAX_HEIGHT))
//            Changed = true;

//        if (filter.Check())
//            Changed = true;
//    }

//    public override bool IsFiltered(TextureGroup item) =>
//        filter?.IsFiltered(item) ?? false ||
//        item.Size.X < MinWidth ||
//        item.Size.X > MaxWidth ||
//        item.Size.Y < MinHeight ||
//        item.Size.Y > MaxHeight;
//}
