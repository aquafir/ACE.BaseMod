namespace OtterGui.Raii;

// Push an arbitrary amount of colors into an object that are all popped when it is disposed.
// If condition is false, no color is pushed.
public static partial class ImRaii
{
    public static Color PushColor(ImGuiCol idx, uint color, bool condition = true)
        => new Color().Push(idx, color, condition);

    public static Color PushColor(ImGuiCol idx, Vector4 color, bool condition = true)
        => new Color().Push(idx, color, condition);

    // Push colors that revert all current color changes made temporarily.
    public static Color DefaultColors()
    {
        var ret          = new Color();
        var reverseStack = Color.Stack.GroupBy(p => p.Item1).Select(p => (p.Key, p.First().Item2)).ToArray();
        foreach (var (idx, val) in reverseStack)
            ret.Push(idx, val);
        return ret;
    }

    public sealed class Color : IDisposable
    {
        internal static readonly List<(ImGuiCol, uint)> Stack = [];
        private                  int                    _count;

        public Color Push(ImGuiCol idx, uint color, bool condition = true)
        {
            if (condition)
            {
                Stack.Add((idx, ImGui.GetColorU32(idx)));
                ImGui.PushStyleColor(idx, color);
                ++_count;
            }

            return this;
        }

        public Color Push(ImGuiCol idx, Vector4 color, bool condition = true)
        {
            if (condition)
            {
                Stack.Add((idx, ImGui.GetColorU32(idx)));
                ImGui.PushStyleColor(idx, color);
                ++_count;
            }

            return this;
        }

        public void Pop(int num = 1)
        {
            num    =  Math.Min(num, _count);
            _count -= num;
            ImGui.PopStyleColor(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose()
            => Pop(_count);
    }
}
