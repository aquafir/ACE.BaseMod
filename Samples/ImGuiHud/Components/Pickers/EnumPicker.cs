public class EnumPicker<T> : IPicker<T> where T : struct, Enum
{
    protected int index = 0;
    protected string[] choices = { };

    /// <summary>
    /// Number of items shown by the combo box
    /// </summary>
    public int ItemsShown = 5;

    /// <summary>
    /// Tracks whether the selection is defined in the enum
    /// </summary>
    public bool Valid;

    /// <summary>
    /// Width of the combo box
    /// </summary>
    public float Width = 120;

    public override void Init()
    {
        choices = Enum.GetNames(typeof(T));

        base.Init();
    }

    public override void DrawBody()
    {
        if (ImGui.Combo(Name, ref index, choices, choices.Length, ItemsShown))
            Changed = true;

        //Parse enum if there's an update, ignore invalid choices?
        if (Changed && index >= 0 && index < choices.Length)
            Valid = Enum.TryParse(choices[index], out Selection);
    }
}

//public class EnumPicker<T> : EnumPicker
//{
//    public EnumPicker() : base(typeof(T)) { }
//}

public class EnumPicker : IPicker<int>
{
    protected int index = 0;
    protected string[] choices = { };

    /// <summary>
    /// Number of items shown by the combo box
    /// </summary>
    public int ItemsShown = 5;

    public Type EnumType;
    public object EnumValue => Enum.ToObject(EnumType, Selection);

    /// <summary>
    /// Width of the combo box
    /// </summary>
    public float Width = 120;

    public EnumPicker(Type type)
    {
        //Check for valid enum
        if (!type.IsEnum)
            throw new ArgumentException($"{type.Name} must be an Enum.");

        this.EnumType = type;

        //Populate choices for combo box
        choices = Enum.GetNames(EnumType);
        //ModManager.Log($"{choices.Length} choices for {EnumType}");
    }

    public override void DrawBody()
    {
        //Separate the choice array to allow for filtered choices
        DrawParseCombo(choices);
    }

    /// <summary>
    /// Render a combo box for a given array of string choices
    /// </summary>
    protected void DrawParseCombo(string[] comboChoices)
    {
        if (ImGui.Combo(Name, ref index, comboChoices, comboChoices.Length, ItemsShown))
            Changed = true;

        //Try to parse enum if there's an update, ignore invalid choices?
        if (Changed && index >= 0 && index < comboChoices.Length)
        {
            try
            {
                Selection = Convert.ToInt32(Enum.Parse(EnumType, comboChoices[index]));
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Failed to parse {EnumType} enum value from the string: {comboChoices[index]}");
            }
        }
    }
}
