

public class FlagsPicker : IPicker<uint>
{
    //Enum name/int values
    protected string[] choiceNames;
    protected uint[] choiceValues;
    //Type of enum
    public Type EnumType;
    public object EnumValue => Enum.ToObject(EnumType, Selection);

    /// <summary>
    /// Width of the combo box
    /// </summary>
    public float Width = 120;

    public FlagsPicker(Type type) : base()
    {
        //Check for valid enum
        //todo: decide on checking for Flags attribute
        if (!type.IsEnum)
            throw new ArgumentException($"{type.Name} must be an Enum.");

        this.EnumType = type;

        //Populate choices
        //choiceNames = Enum.GetNames(EnumType);
        //choiceValues = Enum.GetValues(EnumType).Cast<int>().ToArray();    //Fails with non-ints?

        var values = Enum.GetValues(EnumType);
        choiceNames = new string[values.Length];
        choiceValues = new uint[values.Length];

        int index = 0;
        foreach (var value in values)
        {
            try
            {
                choiceNames[index] = value.ToString();
                choiceValues[index] = Convert.ToUInt32(value);
                index++;
            }catch(Exception ex) { ModManager.Log($"Failed {value}"); }
        }
        ModManager.Log($"{values.Length}->{choiceValues.Length}");
    }

    public override void DrawBody()
    {
        for (var i = 0; i < choiceNames.Length; i++)
        {
            if (ImGui.CheckboxFlags($"{choiceNames[i]}##{_id}", ref Selection, choiceValues[i]))
            {
                //Ctrl click to select only one flag
                if (ImGui.IsKeyDown(ImGuiKey.ModCtrl))
                    Selection = choiceValues[i];

                Changed = true;
            }
        }
    }
}

public class FlagsPicker<T> : FlagsPicker
{
    public FlagsPicker() : base(typeof(T)) { }
}
