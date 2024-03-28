using ImGuiNET;
using System.Text.RegularExpressions;

namespace ImGuiTest;

public class Input
{
    private const ImGuiInputTextFlags Flags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll;
    public static string Text = "";
    public static string TextLabel = "###Query";
    public static string ButtonLabel = "Submit###Query";
    public static bool Focus = false;
    //Automatically compile a regex
    public static Regex Regex = new("");


    /// <summary>
    /// Reusable text input / button that returns true and retains focus on submission
    /// </summary>
    public static bool Render()
    {
        if (Focus)
        {
            ImGui.SetKeyboardFocusHere();
            Focus = false;
        }
        if (ImGui.InputText(TextLabel, ref Text, 1000, Flags))
        {
            Focus = true;
            Regex = new(Text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return true;
        }

        if (ImGui.Button(ButtonLabel))
        {
            Focus = true;
            Regex = new(Text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return true;
        }

        return false;
    }
}