
//public class TextureGroupPicker : ICollectionPicker<TextureGroup>
//{
//    TextureGroupFilter filter = new() { Active = false, Label = "Filter Groups?" };

//    string[] choiceCombo;

//    public override void Init()
//    {
//        UpdateChoices();
//        base.Init();
//    }

//    void UpdateChoices()
//    {
//        var c = TextureManager.TextureGroups.Select(x => new TextureGroup(x.Key, x.Value));
//        Choices = filter.GetFiltered(c).OrderByDescending(x => x.Ids.Count);
//        //.OrderBy(x => x.Size.X * x.Size.Y);
//        choiceCombo = Choices.Select(x => x.ToString()).ToArray();

//        ModManager.Log($"{choiceCombo.Length}");
//    }

//    public override void DrawBody()
//    {
//        if (filter.Check())
//            UpdateChoices();

//        var height = ImGui.GetTextLineHeightWithSpacing() * 5;
//        var size = new Vector2(-1, height);
//        if (ImGui.BeginListBox(Name, size))
//        {
//            for (var i = 0; i < choiceCombo.Length; i++)
//            {
//                bool open;
//                if (ImGui.Selectable(choiceCombo[i]))
//                {
//                    SelectOnly(Choices.ElementAt(i)); //Selection = Choices.ElementAt(i);
//                    ModManager.Log($"Selected {Selection}");
//                    Changed = true;
//                }
//            }

//            ImGui.EndListBox();
//        }
//    }
//}

//public class TextureGroup(Vector2 Size, List<uint> Ids)
//{
//    public readonly Vector2 Size = Size;
//    public readonly List<uint> Ids = Ids;

//    public override string ToString() => $"{Size.X}x{Size.Y} - {Ids?.Count}";
//}
