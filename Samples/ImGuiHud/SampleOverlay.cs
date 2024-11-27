using ACE.DatLoader.Entity;
using ACE.DatLoader.FileTypes;
using ACE.Server.WorldObjects;
using ClickableTransparentOverlay;
using ImGuiHud.Components.Pickers;
using System.Runtime.CompilerServices;
using System.Xml;

namespace ImGuiHud;

public class SampleOverlay : Overlay
{
    //private volatile State state;
    private readonly Thread logicThread;

    //EnumPicker<MotionStance> stance = new();
    PlayerPicker playerPicker = new();
    PickerModal<Player> pickerModal;
    EnumPicker<PaletteTemplate> palettePicker = new();
    //PickerModal<PaletteTemplate> paletteModal = new(palettePicker);
    FlagsPicker<UiEffects> uiEffectsPicker = new();

    //State
    Player p;
    WorldObject wo;
    ClothingTableEx ct;

    public SampleOverlay() : base(1920, 1080)
    {
        pickerModal = new(playerPicker);
        p = PlayerManager.GetAllOnline().FirstOrDefault();
    }

    protected override void Render()
    {
        //ImGui.SetNextWindowPos(new Vector2(0f, 0f));
        ImGui.SetNextWindowSizeConstraints(new(500), new(1000, 800));
        //ImGui.SetNextWindowBgAlpha(0.9f);
        ImGui.Begin("Overly", ImGuiWindowFlags.AlwaysAutoResize);

        RenderMenu();
        ImGui.Separator();

        RenderTabs();

        ImGui.Separator();
        ImGui.End();

        return;
    }

    private void RenderMenu()
    {
        //Select player
        if (ImGui.Button($"{(p is null ? "Select Player" : p?.Name)}"))
        {
            playerPicker.Choices = PlayerManager.GetAllOnline();
            pickerModal.Open();
        }

        if (pickerModal.Check())
        {
            p = playerPicker?.Selection;
        }

        //Check selected object
        if (p is null || !p.TryGetCurrentSelection(out wo))
            return;

        ImGui.SameLine();
        ImGui.Text($"Selected: {wo?.Name}");
    }

    private void RenderTabs()
    {
        if (p is null || !p.TryGetCurrentSelection(out var wo))
            return;

        if (ImGui.BeginTabBar("Tabs"))
        {
            if (wo.PaletteTemplate.HasValue && ImGui.BeginTabItem("Palette"))
            {
                ImGui.Text("This is the content of Tab 1");
                ImGui.EndTabItem();
            }

            if (wo.UiEffects.HasValue && ImGui.BeginTabItem("FX"))
            {
                if(uiEffectsPicker.Check())
                {
                    p.SendMessage($"Selected {wo.UiEffects}");
                }

                ImGui.EndTabItem();
            }

            if(wo.ClothingBase.HasValue && ImGui.BeginTabItem("Clothing"))
            {
                //Check selected ClothingTable
                if (ImGui.Button($"Edit ClothingTable"))
                {
                    if (DatManager.PortalDat.ReadFromDat<ClothingTable>(wo.ClothingBase.Value) is ClothingTable clothingTable)
                        ct = clothingTable.ToClothingTableEx();
                }

                RenderClothingTable();
                ImGui.EndTabItem();
            }

            // End the tab bar
            ImGui.EndTabBar();
        }
        //RenderBody();

    }

    private void RenderBody()
    {
        if (ImGui.BeginChild("Child Window", new System.Numerics.Vector2(-1, 300), ImGuiChildFlags.AlwaysAutoResize))
        {
            RenderPlayer();
            ImGui.EndChild();
        }
    }

    private void RenderPlayer()
    {
        if (p is null)
            return;

        RenderSelection();
    }

    private void RenderSelection()
    {
        if (!p.TryGetCurrentSelection(out var wo))
            return;

        //foreach(var item in )
        ImGui.Text($"Select {wo.Name}");

        RenderPaletteTemplate(wo);

        RenderClothingTable();
    }

    private void RenderPaletteTemplate(WorldObject wo)
    {
        //if (ImGui.Button($"{(PaletteTemplate)wo.PaletteTemplate}"))
        //    paletteModal.Open();

        //if (paletteModal.Check() && paletteModal.Changed)
        //{
        //    p.SendMessage($"Changed {wo.Name} PaletteTemplate from {(PaletteTemplate)wo.PaletteTemplate} to {paletteModal.Selection}");
        //    wo.PaletteTemplate = (int)paletteModal.Selection;
        //    p.EnqueueBroadcast(new GameMessageObjDescEvent(p));
        //}
    }

    private unsafe void RenderClothingTable()
    {
        if (ct is null)
            return;

        //ClothingTable
        //  ClothingBaseEffectEx (Dic)
        //      CloObjectEffect (List)
        //          Index (uint)
        //          ModelId (uint)
        //          CloTextureEffect (List)
        //              OldTexture (uint)
        //              NewTexture (uint)
        //  CloSubPalEffectEx (Dic)
        //      Icon (uint)
        //      CloSubPalette (List)
        //          CloSubPaletteRange (List)
        //              Offset (uint)
        //              NumColors (uint)        
        //ImGui.InputScalar("", ImGuiDataType.U32, )
        int id = 0;
        int value = 0;
        if (ImGui.TreeNode($"ClothingTable {ct.Id:X}##{id++}"))
        {
            ImGui.Text($"ClothingBaseEffects ({ct.ClothingBaseEffects.Count})");
            foreach (var x in ct.ClothingBaseEffects)
            {
                if (ImGui.TreeNode($"{x.Key:X}##{id++}"))
                {
                    foreach (var y in x.Value.CloObjectEffects)
                    {
                        //ImGui.InputScalar($"Index {y.Index}", ImGuiDataType.S32, (IntPtr)Unsafe.AsPointer(ref y.Index), nint.Zero, nint.Zero, "04X", ImGuiInputTextFlags.EnterReturnsTrue);
                        //ImGui.Text($"Index {y.Index}");
                        //ImGui.Text($"ModelId {y.ModelId}");
                        if (ImGui.InputInt($"Index {y.Index}", ref y.Index))
                            Dirty();
                        if (ImGui.InputInt($"ModelId {y.ModelId}", ref y.ModelId))
                            Dirty();

                        if (ImGui.TreeNode($"CloTextureEffects {y.CloTextureEffects.Count}##{id++}"))
                        {
                            foreach (var z in y.CloTextureEffects)
                            {
                                //ImGui.Text($"OldTexture {z.OldTexture:X}");
                                //ImGui.Text($"NewTexture {z.NewTexture:X}");
                                if (ImGui.InputInt($"OldTexture {z.OldTexture:X}", ref z.OldTexture))
                                    Dirty();
                                if (ImGui.InputInt($"NewTexture {z.NewTexture:X}", ref z.NewTexture))
                                    Dirty();
                            }
                            ImGui.TreePop();
                        }
                    }
                    ImGui.TreePop();
                }
            }

            ImGui.Text($"ClothingSubPalEffects ({ct.ClothingSubPalEffects.Count})");
            foreach (var x in ct.ClothingSubPalEffects)
            {
                //ImGui.Text($"Icon {x.Value.Icon:X}");
                if (ImGui.InputInt($"Icon {x.Value.Icon:X}", ref x.Value.Icon))
                    Dirty();

                if (ImGui.TreeNode($"{x.Key:X}##{id++}"))
                {
                    foreach (var y in x.Value.CloSubPalettes)
                    {
                        //ImGui.Text($"PaletteSet {y.PaletteSet:X}");
                        if (ImGui.InputInt($"PaletteSet {y.PaletteSet:X}", ref y.PaletteSet))
                            Dirty();

                        if (ImGui.TreeNode($"Ranges {y.Ranges.Count}##{id++}"))
                        {
                            foreach (var z in y.Ranges)
                            {
                                //ImGui.Text($"NumColors {z.NumColors}");
                                //ImGui.Text($"Offset {z.Offset:X}");
                                if (ImGui.InputInt($"NumColors {z.NumColors}", ref z.NumColors))
                                    Dirty();
                                if (ImGui.InputInt($"Offset {z.Offset:X}", ref z.Offset))
                                    Dirty();
                            }
                            ImGui.TreePop();
                        }
                    }
                    ImGui.TreePop();
                }
            }
            ImGui.TreePop();
        }
    }

    private void Dirty()
    {
        p?.SendMessage($"Dirty");
        var newTable = ct.Convert();
        //DatManager.PortalDat.ReadFromDat<ClothingTable>(wo.ClothingBase.Value) is ClothingTable clothingTable)
        Debugger.Break();
        //Clear
        DatManager.PortalDat.FileCache.TryRemove(ct.Id, out var value);
        DatManager.PortalDat.FileCache.TryAdd(ct.Id, newTable);

        if (p is null || wo is null)
            return;

        if (wo.CurrentWieldedLocation != null)
            p.EnqueueBroadcast(new GameMessageObjDescEvent(p));
    }

    public override void Close()
    {
        base.Close();
        //this.state.IsRunning = false;
    }
}


public record struct SimulatedSwing(MotionStance stance, MotionCommand command, float time);
