using Avalonia;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;

namespace NumericUpDownLib.Avalonia.Base;

/// <summary>
/// This class serves as a target for styling the <see cref="AbstractBaseUpDown{T}"/> class
/// since styling directly on <see cref="AbstractBaseUpDown{T}"/> is not supported in XAML.
/// </summary>
public abstract class InputBaseUpDown : TemplatedControl
{
    #region fields
    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<InputBaseUpDown, bool>(nameof(IsReadOnly),
            defaultValue: (true));

    /// <summary>
    /// Determines the allowed style of a number entered and displayed in the textbox.
    /// </summary>
    public static readonly StyledProperty<NumberStyles> NumberStyleProperty =
        AvaloniaProperty.Register<InputBaseUpDown, NumberStyles>(nameof(NumberStyle), defaultValue: (NumberStyles.Any));

    /// <summary>
    /// Backing store of <see cref="EnableValidatingIndicator"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> EnableValidatingIndicatorProperty =
        AvaloniaProperty.Register<InputBaseUpDown, bool>(nameof(EnableValidatingIndicator), defaultValue: (false));

    /// <summary>
    /// Backing store of <see cref="EditingVisibility"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ScrollBarVisibility> EditingVisibilityProperty =
        AvaloniaProperty.Register<InputBaseUpDown, ScrollBarVisibility>(nameof(EditingVisibility), defaultValue: ScrollBarVisibility.Hidden);

    /// <summary>
    /// Backing store of <see cref="EditingColorBrush"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<SolidColorBrush> EditingColorBrushProperty =
        AvaloniaProperty.Register<InputBaseUpDown, SolidColorBrush>(nameof(EditingColorBrush), defaultValue: SolidColorBrush.Parse("#ff008000"));


    /// <summary>
    /// identify that the inputing data is valid or not.,
    /// </summary>
    /// <value></value>
    public SolidColorBrush EditingColorBrush
    {
        get
        {
            return GetValue(EditingColorBrushProperty);
        }
        protected set
        {
            SetValue(EditingColorBrushProperty, value);
        }
    }

    /// <summary>
    /// identify that the editing Visibility
    /// </summary>
    /// <value></value>
    public ScrollBarVisibility EditingVisibility
    {
        get
        {
            return (ScrollBarVisibility)GetValue(EditingVisibilityProperty);
        }
        protected set
        {
            SetValue(EditingVisibilityProperty, value);
        }
    }

    /// <summary>
    /// identify that the is enable the red/green tip while editing
    /// </summary>
    /// <value></value>
    public bool EnableValidatingIndicator
    {
        get
        {
            return (bool)GetValue(EnableValidatingIndicatorProperty);
        }
        set
        {
            SetValue(EnableValidatingIndicatorProperty, value);
        }
    }

    private static RelayCommand _IncreaseCommand;
    private static RelayCommand _DecreaseCommand;
    #endregion fields

    /// <summary>
    /// Class constructor
    /// </summary>
    public InputBaseUpDown()
    {
        InitializeCommands();
    }

    #region properties
    /// <summary>
    /// Expose the increase value command via <seealso cref="RelayCommand"/> property.
    /// </summary>
    public static RelayCommand IncreaseCommand
    {
        get
        {
            return _IncreaseCommand;
        }
    }

    /// <summary>
    /// Expose the decrease value command via <seealso cref="RelayCommand"/> property.
    /// </summary>
    public static RelayCommand DecreaseCommand
    {
        get
        {
            return _DecreaseCommand;
        }
    }

    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            return (bool)GetValue(IsReadOnlyProperty);
        }
        set
        {
            SetValue(IsReadOnlyProperty, value);
        }
    }

    /// <summary>
    /// Gets/sets the allowed style of a number entered and displayed in the textbox.
    /// </summary>
    public NumberStyles NumberStyle
    {
        get
        {
            return (NumberStyles)GetValue(NumberStyleProperty);
        }
        set
        {
            SetValue(NumberStyleProperty, value);
        }
    }

    #endregion properties

    #region methods
    #region Commands
    /// <summary>
    /// Increase the displayed integer value
    /// </summary>
    protected abstract void OnIncrease();

    /// <summary>
    /// Determines whether the increase command is available or not.
    /// </summary>
    protected abstract bool CanIncreaseCommand();

    /// <summary>
    /// Decrease the displayed integer value
    /// </summary>
    protected abstract void OnDecrease();

    /// <summary>
    /// Determines whether the decrease command is available or not.
    /// </summary>
    protected abstract bool CanDecreaseCommand();

    /// <summary>
    /// Initialize up down/button commands and key gestures for up/down cursor keys
    /// </summary>
#if true
    private void InitializeCommands()
    {
    }
#else
    private void InitializeCommands()
    {
        InputBaseUpDown._IncreaseCommand = new RelayCommand("IncreaseCommand", typeof(InputBaseUpDown));

        CommandManager.RegisterClassCommandBinding(typeof(InputBaseUpDown),
                                new CommandBinding(_IncreaseCommand, OnIncreaseCommand, OnCanIncreaseCommand));

        CommandManager.RegisterClassInputBinding(typeof(InputBaseUpDown),
                                new InputBinding(_IncreaseCommand, new KeyGesture(Key.Up)));

        InputBaseUpDown._DecreaseCommand = new RelayCommand("DecreaseCommand", typeof(InputBaseUpDown));

        CommandManager.RegisterClassCommandBinding(typeof(InputBaseUpDown),
                                new CommandBinding(_DecreaseCommand, OnDecreaseCommand, OnCanDecreaseCommand));
    }


    /// <summary>
    /// Determine whether the IncreaseCommand can be executed or not and return the result
    /// in the <see cref="CanExecuteRoutedEventArgs.CanExecute"/> property of the given
    /// <paramref name="e"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnCanIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var control = sender as InputBaseUpDown;
        if (control != null)
        {
            e.CanExecute = control.CanIncreaseCommand();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Execute the increase value command
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var control = sender as InputBaseUpDown;
        if (control != null)
        {
            control.OnIncrease();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Execute the decrease value command
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var control = sender as InputBaseUpDown;
        if (control != null)
        {
            control.OnDecrease();
            e.Handled = true;
        }
    }

    /// <summary>
    /// Determine whether the DecreaseCommand can be executed or not and return the result
    /// in the <see cref="CanExecuteRoutedEventArgs.CanExecute"/> property of the given
    /// <paramref name="e"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnCanDecreaseCommand(object sender, CanExecuteRoutedEventArgs e)
    {
        var control = sender as InputBaseUpDown;
        if (control != null)
        {
            e.CanExecute = control.CanDecreaseCommand();
            e.Handled = true;
        }
    }

#endif
    #endregion
    #endregion methods
}