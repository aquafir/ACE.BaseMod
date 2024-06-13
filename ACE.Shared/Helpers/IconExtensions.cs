namespace ACE.Shared.Helpers;
public enum IconUnderlayColor : uint
{
    White       = 0x0600335A,
    Red         = 0x06003359,
    Blue        = 0x06003358,
    DarkRed     = 0x06003357,
    Orange      = 0x06003356,
    Green       = 0x06003355,
    Purple      = 0x06003354,
    LightBlue   = 0x06003353,
}

public enum IconOverlay : uint
{
    //Top-left
    ShortcutA0 = 0x0600109D,
    ShortcutA1 = 0x0600109E,
    ShortcutA2 = 0x0600109F,
    ShortcutA3 = 0x060010A0,
    ShortcutA4 = 0x060010A1,
    ShortcutA5 = 0x060010A2,
    ShortcutA6 = 0x060010A3,
    ShortcutA7 = 0x060010A4,
    ShortcutA8 = 0x060010A5,
    ShortcutA9 = 0x060010A6,

    //Top-left blue
    ShortcutB0 = 0x060019EC,
    ShortcutB1 = 0x060019ED,
    ShortcutB2 = 0x060019EE,
    ShortcutB3 = 0x060019EF,
    ShortcutB4 = 0x060019F0,
    ShortcutB5 = 0x060019F1,
    ShortcutB6 = 0x060019F2,
    ShortcutB7 = 0x060019F3,
    ShortcutB8 = 0x060019F4,
    ShortcutB9 = 0x060019F5,

    //Top-right white
    ShortcutC0 = 0x06006C33,
    ShortcutC1 = 0x06006C34,
    ShortcutC2 = 0x06006C35,
    ShortcutC3 = 0x06006C36,
    ShortcutC4 = 0x06006C37,
    ShortcutC5 = 0x06006C38,
    ShortcutC6 = 0x06006C39,
    ShortcutC7 = 0x06006C3A,
    ShortcutC8 = 0x06006C3B,
    ShortcutC9 = 0x06006C3C,

    //Top-right black - more visible
    ShortcutD0 = 0x06006C1F,
    ShortcutD1 = 0x06006C20,
    ShortcutD2 = 0x06006C21,
    ShortcutD3 = 0x06006C22,
    ShortcutD4 = 0x06006C23,
    ShortcutD5 = 0x06006C24,
    ShortcutD6 = 0x06006C25,
    ShortcutD7 = 0x06006C26,
    ShortcutD8 = 0x06006C27,
    ShortcutD9 = 0x06006C28,
}

public static class IconExtensions
{
    public static void AddIconUnderlay(this WorldObject wo, IconUnderlayColor color) =>
        wo.IconUnderlayId = (uint)color;

    public static void AddIconOverlay(this WorldObject wo, IconOverlay color) =>
        wo.IconOverlayId = (uint)color;

}

