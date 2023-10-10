
namespace NumericUpDownLib.Avalonia;

using global::Avalonia;
using NumericUpDownLib.Avalonia.Base;
public class NumericUpDown : InputBaseUpDown
{
    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<NumericUpDown, int>(nameof(ValueProperty),
            defaultValue: (0));

    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public int Value
    {
        get
        {
            return (int)GetValue(ValueProperty);
        }
        set
        {
            SetValue(ValueProperty, value);
        }
    }

    protected override bool CanDecreaseCommand() => throw new System.NotImplementedException();
    protected override bool CanIncreaseCommand() => throw new System.NotImplementedException();
    protected override void OnDecrease() => throw new System.NotImplementedException();
    protected override void OnIncrease() => throw new System.NotImplementedException();
}
