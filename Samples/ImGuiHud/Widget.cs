
namespace ImGuiTest;

public abstract class Widget
{
    public string Label { get; set; }
    public bool Open = false;

    public virtual void Render() { }

    public Widget(string label) => Label = label;
}
