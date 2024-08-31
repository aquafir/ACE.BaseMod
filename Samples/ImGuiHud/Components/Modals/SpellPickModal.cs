

//public class SpellPickModal : IModal
//{
//    public SpellPicker Picker = new();

//    public override void DrawBody()
//    {
//        if (Picker.Check())
//        {
//            ModManager.Log($"{Picker.Selection.Spell.Name}");
//            Save();
//        }

//        if(Picker.Mode == SelectionStyle.Multiple)
//        {
//            int index = 0;
//            foreach (var item in Picker.Selected)
//                Picker.DrawItem(item, index++);
//        }
//    }
//}
