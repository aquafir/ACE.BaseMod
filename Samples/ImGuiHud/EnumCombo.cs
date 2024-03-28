using ImGuiNET;
using System.Text.RegularExpressions;

namespace ImGuiTest;

public class EnumCombo<TEnum> where TEnum : Enum
{
    private const ImGuiInputTextFlags Flags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll;
    //public string Text = "";
    public string Label = "###Query";
    public int Width { get; set; } = 200;
    //Automatically compile a regex
    public static Regex Regex = new("");

    public EnumCombo()
    {
        Options = Enum.GetNames(typeof(TEnum));
    }




    public int Index = 0;
    public string[] Options;
    public bool Render()
    {
        ImGui.PushItemWidth(Width);
        if (ImGui.Combo(Label, ref Index, Options, Options.Length))
        {

        }

        return false;
    }
}