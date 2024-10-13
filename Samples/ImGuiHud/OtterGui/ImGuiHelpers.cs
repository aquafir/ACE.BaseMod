namespace ImGuiHud.OtterGui;

public static class ImGuiHelpers
{
    public static float GlobalScale { get; set; } = 1;
}


public static class ColorHelpers
{
    /// <summary> Obtain the correct color for a frame depending on mouse state. </summary>
    /// <param name="hovered"> Whether the object is hovered. </param>
    /// <param name="held"> Whether a mouse button is held down on the object. </param>
    /// <returns> The frame color. </returns>
    public static uint GetFrameBg(bool hovered, bool held)
        => ImGui.GetColorU32((hovered, held) switch
        {
            (true, true) => ImGuiCol.FrameBgActive,
            (true, false) => ImGuiCol.FrameBgHovered,
            _ => ImGuiCol.FrameBg,
        });

    /// <summary> Obtain the correct color for a button depending on mouse state. </summary>
    /// <param name="hovered"> Whether the button is hovered. </param>
    /// <param name="held"> Whether a mouse button is held down on the button. </param>
    /// <returns> The frame color. </returns>
    public static uint GetButtonColor(bool hovered, bool held)
        => ImGui.GetColorU32((hovered, held) switch
        {
            (true, true) => ImGuiCol.ButtonActive,
            (true, false) => ImGuiCol.ButtonHovered,
            _ => ImGuiCol.Button,
        });
}


public static class ImUtf8
{
    public static ListClipper ListClipper(int itemsCount, float itemsHeight = -1f)
        => new(itemsCount, itemsHeight);
}

public ref struct ListClipper
{
    /// <summary> The clipper itself. </summary>
    public readonly ImGuiListClipperPtr ClipperPtr;

    public bool Disposed { get; private set; }

    /// <summary> The index of the first displayed element. </summary>
    public readonly ref int DisplayStart
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref ClipperPtr.DisplayStart;
    }

    /// <summary> The index of the last displayed element. </summary>
    public readonly ref int DisplayEnd
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref ClipperPtr.DisplayEnd;
    }

    /// <summary> The number of displayed items. </summary>
    public readonly ref int ItemsCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref ClipperPtr.ItemsCount;
    }

    /// <summary> The total height of the items. </summary>
    public readonly ref float ItemsHeight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref ClipperPtr.ItemsHeight;
    }

    /// <summary> The height offset of the skipped height. </summary>
    public readonly ref float StartPosY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref ClipperPtr.StartPosY;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe ListClipper(int itemsCount, float itemsHeight)
    {
        ClipperPtr = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        ClipperPtr.Begin(itemsCount, itemsHeight);
    }

    /// <summary> Force the current display range using a pair of indices. </summary>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public unsafe void ForceDisplayRangeByIndices(int itemMin, int itemMax)
    //    => ImGuiNative.ImGuiListClipper_ForceDisplayRangeByIndices(ClipperPtr.NativePtr, itemMin, itemMax);

    /// <summary> Execute a step in the clipper. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool Step()
        => ImGuiNative.ImGuiListClipper_Step(ClipperPtr.NativePtr) != 0;

    /// <summary> Dispose of the list clipper. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Dispose()
    {
        if (Disposed)
            return;

        ImGuiNative.ImGuiListClipper_End(ClipperPtr.NativePtr);
        ImGuiNative.ImGuiListClipper_destroy(ClipperPtr.NativePtr);
        Disposed = true;
    }
}

