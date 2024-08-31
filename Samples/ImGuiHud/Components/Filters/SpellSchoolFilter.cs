

//public class SpellSchoolFilter : IOptionalFilter<SpellInfo>
//{
//    public static readonly string[] SchoolNames = //Enum.GetNames(typeof(MagicSchool));
//    {
//       "War", "Life", "Item", "Creature", "Void"
//    };
//    public bool[] Schools = SchoolNames.Select(x => true).ToArray();

//    public override void DrawBody()
//    {
//        for (var i = 0; i < SchoolNames.Length; i++)
//        {
//            ImGui.SameLine();
//            if (ImGui.Checkbox($"{SchoolNames[i]}", ref Schools[i]))
//                Changed = true;
//        }
//    }

//    public override bool IsFiltered(SpellInfo item)
//    {
//        int num = (int)item.Spell.School - 1;    //0 is Unknown
//        return num >= 0 && num < Schools.Length && !Schools[num];
//    }
//}
