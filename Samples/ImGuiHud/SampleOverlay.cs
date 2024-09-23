using ClickableTransparentOverlay;
using ClickableTransparentOverlay.Win32;

namespace ImGuiHud;

public class SampleOverlay : Overlay
{
    private volatile State state;
    private readonly Thread logicThread;

    public SampleOverlay() : base(3840, 2160)
    {
        state = new State();
        logicThread = new Thread(() =>
        {
            var lastRunTickStamp = state.Watch.ElapsedTicks;

            while (state.IsRunning)
            {
                var currentRunTickStamp = state.Watch.ElapsedTicks;
                var delta = currentRunTickStamp - lastRunTickStamp;
                LogicUpdate(delta);
                lastRunTickStamp = currentRunTickStamp;
            }
        });

        logicThread.Start();
    }

    public override void Close()
    {
        base.Close();
        state.IsRunning = false;
    }

    private void LogicUpdate(float updateDeltaTicks)
    {
        state.LogicTicksCounter.Increment();
        state.LogicalDelta = updateDeltaTicks;

        if (state.RequestLogicThreadSleep)
        {
            Thread.Sleep(TimeSpan.FromSeconds(state.SleepInSeconds));
            state.RequestLogicThreadSleep = false;
        }

        if (state.LogicThreadCloseOverlay)
        {
            Close();
            state.LogicThreadCloseOverlay = false;
        }

        //state.OverlaySample2.Update();
        Thread.Sleep(state.LogicTickDelayInMilliseconds); //Not accurate at all as a mechanism for limiting thread runs
    }

    protected override void Render()
    {
        var deltaSeconds = ImGui.GetIO().DeltaTime;

        if (!state.Visible)
        {
            state.ReappearTimeRemaining -= deltaSeconds;
            if (state.ReappearTimeRemaining < 0)
            {
                state.Visible = true;
            }

            return;
        }

        state.RenderFramesCounter.Increment();

        if (Utils.IsKeyPressedAndNotTimeout(VK.F12)) //F12.
        {
            state.ShowClickableMenu = !state.ShowClickableMenu;
        }

        if (state.ShowImGuiDemo)
        {
            ImGui.ShowDemoWindow(ref state.ShowImGuiDemo);
        }

        if (state.ShowOverlaySample1)
        {
            RenderOverlaySample1();
        }

        //if (state.OverlaySample2.Show)
        //{
        //    state.OverlaySample2.Render();
        //}

        if (state.ShowClickableMenu)
        {
            RenderMainMenu();
        }

        return;
    }

    private void RenderMainMenu()
    {
        bool isCollapsed = !ImGui.Begin(
            "Overlay Main Menu",
            ref state.IsRunning,
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize);

        if (!state.IsRunning || isCollapsed)
        {
            ImGui.End();
            if (!state.IsRunning)
            {
                Close();
            }

            return;
        }

        ImGui.Text("Try pressing F12 button to show/hide this menu.");
        ImGui.Text("Click X on top right of this menu to close the overlay.");
        ImGui.Checkbox("Show non-clickable transparent overlay Sample 1.", ref state.ShowOverlaySample1);
        //ImGui.Checkbox("Show full-screen non-clickable transparent overlay sample 2.", ref state.OverlaySample2.Show);

        ImGui.NewLine();
        ImGui.SliderInt2("Set Position", ref state.resizeHelper[0], 0, 3840);
        ImGui.SliderInt2("Set Size", ref state.resizeHelper[2], 0, 3840);
        if (ImGui.Button("Resize"))
        {
            Position = new(state.resizeHelper[0], state.resizeHelper[1]);
            Size = new(state.resizeHelper[2], state.resizeHelper[3]);
        }

        ImGui.NewLine();
        ImGui.SliderInt("###time(sec)", ref state.Seconds, 1, 30);
        if (ImGui.Button($"Hide for {state.Seconds} seconds"))
        {
            state.Visible = false;
            state.ReappearTimeRemaining = state.Seconds;
        }

        ImGui.NewLine();
        ImGui.SliderInt("###sleeptime(sec)", ref state.SleepInSeconds, 1, 30);
        if (ImGui.Button($"Sleep Render Thread for {state.SleepInSeconds} seconds"))
        {
            Thread.Sleep(TimeSpan.FromSeconds(state.SleepInSeconds));
        }

        if (ImGui.Button($"Sleep Logic Thread for {state.SleepInSeconds} seconds"))
        {
            state.RequestLogicThreadSleep = true;
        }

        ImGui.NewLine();
        if (ImGui.Button($"Request Logic Thread to close Overlay."))
        {
            state.LogicThreadCloseOverlay = true;
        }

        ImGui.NewLine();
        ImGui.SliderInt("Logical Thread Delay(ms)", ref state.LogicTickDelayInMilliseconds, 1, 1000);
        ImGui.NewLine();
        if (ImGui.Button("Toggle ImGui Demo"))
        {
            state.ShowImGuiDemo = !state.ShowImGuiDemo;
        }

        ImGui.NewLine();
        if (File.Exists("image.png"))
        {
            AddOrGetImagePointer(
                "image.png",
                false,
                out nint imgPtr,
                out uint w,
                out uint h);
            ImGui.Image(imgPtr, new Vector2(w, h));
        }
        else
        {
            ImGui.Text("Put any image where the exe is, name is 'image.png'");
        }

        ImGui.End();
    }

    private void RenderOverlaySample1()
    {
        ImGui.SetNextWindowPos(new Vector2(0f, 0f));
        ImGui.SetNextWindowBgAlpha(0.9f);
        ImGui.Begin(
            "Sample Overlay",
            ImGuiWindowFlags.NoInputs |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoResize);

        ImGui.Text("I am sample Overlay to display stuff.");
        ImGui.Text("You can not click me.");
        ImGui.NewLine();
        ImGui.Text($"Number of displays {NumberVideoDisplays}");
        ImGui.Text($"Current Date: {DateTime.Now.Date.ToShortDateString()}");
        ImGui.Text($"Current Time: {DateTime.Now.TimeOfDay}");
        ImGui.Text($"Total Rendered Frames: {state.RenderFramesCounter.Count}");
        ImGui.Text($"Render Delta (seconds): {ImGui.GetIO().DeltaTime:F4}");
        ImGui.Text($"Render FPS: {ImGui.GetIO().Framerate:F1}");
        ImGui.Text($"Total Logic Frames: {state.LogicTicksCounter.Count}");
        ImGui.Text($"Logic Delta (seconds): {state.LogicalDelta / Stopwatch.Frequency:F4}");
        ImGui.End();
    }
}


public class State
{
    public readonly Stopwatch Watch = Stopwatch.StartNew();
    public bool ShowClickableMenu = true;
    public bool ShowOverlaySample1 = true;
    public bool ShowImGuiDemo = false;
    public int[] resizeHelper = new int[4] { 0, 0, 2560, 1440 };
    public int Seconds = 5;
    public int CurrentDisplay = 0;
    public bool IsRunning = true;
    public bool Visible;


    public int LogicTickDelayInMilliseconds = 10;
    public float LogicalDelta = 0;
    public readonly Counter RenderFramesCounter = new Counter();
    public readonly Counter LogicTicksCounter = new Counter();
    public bool RequestLogicThreadSleep = false;
    public int SleepInSeconds = 5;


    public float ReappearTimeRemaining = 0;

    public bool LogicThreadCloseOverlay = false;
}

public class Counter
{
    public long Count { get; private set; }

    public void Increment()
    {
        Count++;
    }
}