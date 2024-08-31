
public class ValueComparisonFilter<T> : IOptionalFilter<T>
{
    protected double value;
    protected readonly Func<T, double?> targetPredicate;

    public ValueComparisonFilter(Func<T, double?> targetPredicate) : base(null)
    {
        this.targetPredicate = targetPredicate; //?? throw new ArgumentNullException(nameof(targetPredicate));
    }

    FilteredEnumPicker<CompareType> comparison = new() { Label = "Comparison" };
    public override void DrawBody()
    {
        if (comparison.Check())
        {
            Changed = true;
            ModManager.Log($"{comparison.Selection}");
        }

        ImGui.SameLine();
        if (ImGui.InputDouble($"Value##{_id}", ref value, .1, .5, value.ToString(), ImGuiInputTextFlags.AutoSelectAll))
        {
            ModManager.Log($"{value}");
            Changed = true;
        }
    }

    public override bool IsFiltered(T item)
    {
        if (comparison is null)
            return false;

        var comp = targetPredicate(item);
        var result = comparison.Selection.VerifyRequirement(targetPredicate(item), value);
        ModManager.Log($"{comp} {comparison.Selection} {value} -> {result}");
        return comparison.Selection.VerifyRequirement(targetPredicate(item), value);
    }
}
