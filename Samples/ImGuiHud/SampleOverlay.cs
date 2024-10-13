using ClickableTransparentOverlay;
using ImGuiHud.Components.Pickers;

namespace ImGuiHud;

public class SampleOverlay : Overlay
{
    //private volatile State state;
    private readonly Thread logicThread;

    EnumPicker<MotionStance> stance = new();
    EnumPicker<MotionCommand> command = new();
    EnumPicker<MovementType> movement = new();

    PlayerPicker playerPicker = new();
    PickerModal<Player> pickerModal;
    Spell spell;
    int spellId;
    Player p => playerPicker?.Selection;

    List<SimulatedSwing> swingSims = new();

    public SampleOverlay() : base(1920, 1080)
    {
        pickerModal = new(playerPicker);

        //Based on SampleOverlay, which has a background logic thread example if needed
        //https://github.com/zaafar/ClickableTransparentOverlay/blob/master/Examples/MultiThreadedOverlay/SampleOverlay.cs
        //state = new State();
    }

    protected override void Render()
    {
        //ImGui.SetNextWindowPos(new Vector2(0f, 0f));
        ImGui.SetNextWindowSizeConstraints(new(500), new(1000));
        //ImGui.SetNextWindowBgAlpha(0.9f);
        ImGui.Begin("Overly", ImGuiWindowFlags.AlwaysAutoResize);

        RenderMainMenu();
        ImGui.Separator();

        RenderBody();

        ImGui.End();

        return;
    }

    private void RenderBody()
    {
        stance.Check();
        command.Check();
        movement.Check();

        RenderMelee();

        RenderSpell();

        if (stance.Selection == default && command.Selection == default)
        {
            ImGui.Text($"Select stuff?");
            return;
        }


    }

    Table<SimulatedSwing> meleeSwings;
    private void RenderMelee()
    {
        if (p is null)
            return;

        if (ImGui.Button($"Melee Speeds - {swingSims.Count}"))
        {
            swingSims = new();
            foreach (var command in Enum.GetValues<MotionCommand>())
            {
                foreach (var stance in Enum.GetValues<MotionStance>())
                {
                    var speed = SimulationExtensions.GetSimulatedMeleeDelay(command, stance);
                    if (speed > 0)
                        swingSims.Add(new(stance, command, speed));
                }
            }

            //meleeSwings = new("Swings", swingSims, new[]
            //{
            //    new Column<SimulatedSwing>() {  Label = "S"}
            //});

        }

        if (swingSims.Count == 0)
            return;

        //meleeSwings.Draw(400);
        ImGuiTable.DrawTable("Table", swingSims, x =>
        {
            ImGui.TableNextColumn();
            ImGui.Text($"{x.command}");
            ImGui.TableNextColumn();
            ImGui.Text($"{x.stance}");
            ImGui.TableNextColumn();
            ImGui.Text($"{x.time}");
        }, ImGuiTableFlags.Borders | ImGuiTableFlags.Sortable, "Command", "Stance", "Time");

        //foreach(var sim in swingSims)
        //{
        //    ImGui.Text($"");
        //}
    }



    private void RenderSpell()
    {
        if (ImGui.DragInt("SpellID", ref spellId, 1, 1, 6666))
        {
            spell = new(spellId);
            if (spell.NotFound || spell.Formula is null)
                spell = null;
        }

        if (spell is not null)
        {
            ImGui.Text($"Spell: {spell?.Name}");
            ImGui.Text($"Time: {spell.Formula.GetCastTime(0x09000001, 1)}");
        }
    }

    private void RenderMainMenu()
    {

        if (ImGui.Button($"{(p is null ? "Select Player" : p?.Name)}"))
        {
            playerPicker.Choices = PlayerManager.GetAllOnline();
            pickerModal.Open();
        }

        pickerModal.Check();
    }


    public override void Close()
    {
        base.Close();
        //this.state.IsRunning = false;
    }
}


public record struct SimulatedSwing(MotionStance stance, MotionCommand command, float time);

//public class State
//{
//    public bool IsRunning = true;
//    public bool Visible;
//    public Player Player;
//    public WorldObject Selected;
//}
