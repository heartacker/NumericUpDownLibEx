namespace NumericUpDownLib.WinUI.Base;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using NumericUpDownLib.Enums;
using NumericUpDownLib.Models;
using NumericUpDownLib.WinUI.Helper;
using System;
using System.ComponentModel;
using System.Diagnostics;
//using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;

using System.Windows.Input;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;

/// <summary>
/// Implements an up/down abstract base control.
/// Source: http://msdn.microsoft.com/en-us/library/vstudio/ms771573%28v=vs.90%29.aspx
/// </summary>
///
[TemplatePart(Name = Part_TextBoxName, Type = typeof(TextBox))]
[TemplatePart(Name = PART_MeasuringElement, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PART_IncrementButton, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_DecrementButton, Type = typeof(RepeatButton))]
[TemplatePart(Name = PART_IncDecStackPanel, Type = typeof(StackPanel))]
public abstract partial class AbstractBaseUpDown<T> : InputBaseUpDown/* TODO, ICommandSource*/
{
    #region fields
    /// <summary>
    /// Gets the required template name of the textbox portion of this control.
    /// </summary>
    public const string Part_TextBoxName = "PART_TextBox";

    /// <summary>
    /// Gets the required template name of the textbox portion of this control.
    /// </summary>
    public const string PART_MeasuringElement = "PART_Measuring_Element";

    /// <summary>
    /// Gets the required template name of the textbox portion of this control.
    /// </summary>
    public const string PART_IncDecStackPanel = "PART_IncDecStackPanel";

    /// <summary>
    /// Gets the required template name of the increment button for this control.
    /// </summary>
    public const string PART_IncrementButton = "PART_IncrementButton";

    /// <summary>
    /// Gets the required template name of the decrement button for this control.
    /// </summary>
    public const string PART_DecrementButton = "PART_DecrementButton";

    /// <summary>
    /// Gets/sets the default applicable minimum value
    ///
    /// Set this value in the static constructor of an inheriting class if a different
    /// default format string is more appropriate in the context of that inheriting class.
    /// </summary>
    protected static T _MinValue = default(T);

    /// <summary>
    /// Gets/sets the default applicable maximum value
    ///
    /// Set this value in the static constructor of an inheriting class if a different
    /// default format string is more appropriate in the context of that inheriting class.
    /// </summary>
    protected static T _MaxValue = default(T);

    #endregion


    #region constructor
    /// <summary>
    /// Static class constructor
    /// </summary>
    static AbstractBaseUpDown()
    {
        // TODO DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractBaseUpDown<T>),
        //    new FrameworkPropertyMetadata(typeof(AbstractBaseUpDown<T>)));
    }

    /// <summary>
    /// Initializes a new instance of the AbstractBaseUpDown Control.
    /// </summary>
    public AbstractBaseUpDown() : base()
    {
        this.DefaultStyleKey = typeof(AbstractBaseUpDown<T>);
        UserInput = false;
        HorizontalContentAlignment = HorizontalAlignment.Right;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        this.Loaded += AbstractBaseUpDown_Loaded;
        this.Unloaded += AbstractBaseUpDown_Unloaded;
    }

    private void AbstractBaseUpDown_Loaded(object sender, RoutedEventArgs e)
    {
        if (_PART_TextBox != null && DependencyPropertyHelper.FindChildrenOfType<Button>(_PART_TextBox).Where(x => x.Name == nameof(DeleteButton))
            .FirstOrDefault() is Button deleteButton)
        {
            DeleteButton = deleteButton;
            OnDeleteButtonVisibilityPropertyChanged(null, null);
            DeleteButtonPropertyChangedCallbackToken = DeleteButton.RegisterPropertyChangedCallback(
                VisibilityProperty,
                OnDeleteButtonVisibilityPropertyChanged);
        }
    }

    private void AbstractBaseUpDown_Unloaded(object sender, RoutedEventArgs e)
    {
        UnregisterPropertyChangedCallback(VisibilityProperty, DeleteButtonPropertyChangedCallbackToken);
    }




    /// <summary>
    /// Is invoked whenever application code or internal processes call
    /// System.Windows.FrameworkElement.ApplyTemplate.
    /// </summary>

    private Button? DeleteButton
    {
        get; set;
    }
    private long DeleteButtonPropertyChangedCallbackToken
    {
        get; set;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _PART_TextBox = this.GetTemplateChild(Part_TextBoxName) as TextBox;
        _PART_Measuring_Element = this.GetTemplateChild(PART_MeasuringElement) as TextBox;

        _PART_IncDecStackPanel = this.GetTemplateChild(PART_IncDecStackPanel) as StackPanel;

        _PART_DecrementButton = this.GetTemplateChild(PART_DecrementButton) as RepeatButton;
        _PART_IncrementButton = this.GetTemplateChild(PART_IncrementButton) as RepeatButton;


        if (_PART_TextBox != null)
        {
            BindMeasuringObject(IsDisplayLengthFixed);

            FormatText(_PART_TextBox.Text);  // Ensure initial text is according to format

            _PART_TextBox.TextChanging += _PART_TextBox_TextChanging;
            _PART_TextBox.TextChanged += _PART_TextBox_TextChanged;

            _PART_TextBox.PointerEntered += _PART_TextBox_MouseEnter;
            _PART_TextBox.PointerWheelChanged += _PART_TextBox_PointerWheelChanged; ;

#if WPF
            _PART_TextBox.GotKeyboardFocus += _PART_TextBox_GotKeyboardFocus;
            _PART_TextBox.LostKeyboardFocus += _PART_TextBox_LostKeyboardFocus;

            _PART_TextBox.MouseMove += _PART_TextBox_MouseMove;
            _PART_TextBox.MouseUp += _PART_TextBox_MouseUp;
            _PART_TextBox.PreviewMouseDown += _PART_TextBox_PreviewMouseDown;
            _PART_TextBox.LostMouseCapture += _PART_TextBox_LostMouseCapture;

#endif



            _PART_TextBox.PreviewKeyDown += _PART_TextBox_PreviewKeyDown; ;
            _PART_TextBox.Paste += _PART_TextBox_Paste;
            _PART_TextBox.GotFocus += _PART_TextBox_GotFocus;
            _PART_TextBox.LostFocus += _PART_TextBox_LostFocus; ;


            _PART_TextBox.ProcessKeyboardAccelerators += _PART_TextBox_ProcessKeyboardAccelerators;
            _PART_TextBox.SelectionChanged += _PART_TextBox_SelectionChanged;
            _PART_TextBox.SelectionChanging += _PART_TextBox_SelectionChanging;
        }

        if (_PART_DecrementButton != null)
            _PART_DecrementButton.PreviewKeyDown += IncDecButton_PreviewKeyDown;

        if (_PART_IncrementButton != null)
            _PART_IncrementButton.PreviewKeyDown += IncDecButton_PreviewKeyDown;

        if (_PART_Measuring_Element != null)
        {
            _PART_Measuring_Element.TextChanged += _PART_Measuring_Element_TextChanged;
            _PART_Measuring_Element.LayoutUpdated += _PART_Measuring_Element_LayoutUpdated; ;
        }

#if WPF
        this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this_IsVisibleChanged);
#endif
    }

    #endregion constructor


    #region DependencyProperty
    /// <summary>
    /// Dependency property backing store for the Value property. defalut value is _MinValue
    /// </summary>
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(T),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(
            _MinValue, new PropertyChangedCallback(OnValuePropertyChanged)
            //, new CoerceValueCallback(CoerceValue)
            )
        );

    /// <summary>
    /// Dependency property backing store for Minimum Value property.
    /// </summary>
    protected static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
        nameof(MinValue),
            typeof(T), typeof(AbstractBaseUpDown<T>),
            new PropertyMetadata(
                _MinValue,
                new PropertyChangedCallback(OnMinValuePropertyChanged)
                //, new CoerceValueCallback(CoerceMinValue)
                )
            );

    /// <summary>
    /// Dependency property backing store for Maximum Value property.
    /// </summary>
    protected static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
        nameof(MaxValue),
        typeof(T), typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(
            _MaxValue,
            new PropertyChangedCallback(OnMaxValuePropertyChanged)
            //, new CoerceValueCallback(CoerceMaxValue)
            )
        );

    public static readonly DependencyProperty IsDeleteButtonVisibleProperty = DependencyProperty.Register(
        nameof(IsDeleteButtonVisible),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(false));


#if WPF
    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    protected static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(AbstractBaseUpDown<T>));

    /// <summary>
    /// Identifies the MinValueChanged routed event.
    /// </summary>
    protected static readonly RoutedEvent MinValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            "MinValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(AbstractBaseUpDown<T>));

    /// <summary>
    /// Identifies the MaxValueChanged routed event.
    /// </summary>
    protected static readonly RoutedEvent MaxValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            "MaxValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(AbstractBaseUpDown<T>));

#endif

    /// <summary>
    /// Backing store for dependency property to define the number of characters
    /// that should be displayed in the control without having to scroll inside
    /// the textbox portion.
    /// </summary>
    protected static readonly DependencyProperty DisplayLengthProperty = DependencyProperty.Register(
        nameof(DisplayLength),
        typeof(int),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata((int)3, new PropertyChangedCallback(DisplayLengthPropertyChanged)
            ));


    /// <summary>
    /// Backing store for dependency property to decide whether DisplayLength
    /// definition leads to a fixed control size (textbox control will scroll
    /// if user types longer string), or not (control will resize in dependence
    /// of string length and available space).
    /// </summary>
    protected static readonly DependencyProperty IsDisplayLengthFixedProperty = DependencyProperty.Register(
        nameof(IsDisplayLengthFixed),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(true, OnIsDisplayLengthFixedChanged));

    /// <summary>
    /// Backing store for dependency property to decide whether all text in textbox
    /// should be selected upon focus or not.
    /// </summary>
    protected static readonly DependencyProperty SelectAllTextOnFocusProperty = DependencyProperty.Register(
        nameof(SelectAllTextOnFocus),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(false));

    /// <summary>
    /// Backing store for dependency property for .Net FormatString that is
    /// applied to the textbox text portion of the up down control.
    /// </summary>
    protected static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register(
        nameof(FormatString),
        typeof(string),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata("G", OnFormatStringChanged));

    /// <summary>
    /// Backing store of <see cref="MouseWheelAccelaratorKey"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MouseWheelAccelaratorKeyProperty = DependencyProperty.Register(
        nameof(MouseWheelAccelaratorKey),
        typeof(VirtualKeyModifiers),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(VirtualKeyModifiers.Control));

    /// <summary>
    /// Backing store of <see cref="IsMouseDragEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsMouseDragEnabledProperty = DependencyProperty.Register(
        nameof(IsMouseDragEnabled),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(true, OnIsMouseDragEnabledChanged));

    /// <summary>
    /// Backing store of <see cref="CanIncDecMouseDrag"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanMouseDragProperty = DependencyProperty.Register(
        nameof(CanMouseDrag),
        typeof(CanIncDecMouseDrag),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(CanIncDecMouseDrag.VerticalHorizontal));

    public static readonly DependencyProperty MouseWheelEnabledProperty = DependencyProperty.Register(
        nameof(MouseWheelEnabled),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(true));

    /// <summary>
    /// Backing store of <see cref="IsLargeChangeEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsLargeStepEnabledProperty = DependencyProperty.Register(
        nameof(IsLargeChangeEnabled),
        typeof(bool),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(false));

    /// <summary>
    /// Backing store of <see cref="IsUpdateValueWhenLostFocus"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsUpdateValueWhenLostFocusProperty = DependencyProperty.Register(
            nameof(IsUpdateValueWhenLostFocus),
            typeof(bool),
            typeof(AbstractBaseUpDown<T>),
            new PropertyMetadata(false));

    /// <summary>
    /// Backing store of <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewOrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(AbstractBaseUpDown<T>),
            new PropertyMetadata(Orientation.Horizontal));
    #endregion

    #region field

    /// <summary>
    /// Holds the REQUIRED textbox instance part for this control.
    /// </summary>
    protected TextBox _PART_TextBox;

    /// <summary>
    /// Measures the required space for a string of a certain length
    /// with a standard control to ensure that enough digits are visible.
    /// </summary>
    private TextBox _PART_Measuring_Element;

    private StackPanel _PART_IncDecStackPanel;

    private RepeatButton _PART_DecrementButton;
    private RepeatButton _PART_IncrementButton;

    private MouseIncrementor _objMouseIncr;
    #endregion fields


    #region events

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<T>> MinValueChanged;

    /// <summary>
    /// Identifies the ValueChanged routed event.
    /// </summary>
    public event EventHandler<ValueChangedEventArgs<T>> MaxValueChanged;

    #endregion events

    #region Command
    /// <summary>
    /// Gets/Sets a command that can be invoked when a up/down button is clicked.
    /// </summary>
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Dependency property backing store for Command Value property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        nameof(Command),
        typeof(XamlUICommand),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(null, new PropertyChangedCallback(CommandChangedCallBack)));

    private static void CommandChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AbstractBaseUpDown<T> nud)
        {
            var oldCommand = e.OldValue as XamlUICommand;
            var newCommand = e.NewValue as XamlUICommand;
            nud.HookUpCommand(oldCommand, newCommand);
        }
    }

    /// <summary>
    /// Dependency property backing store for CommandParameter Value property.
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
        nameof(CommandParameter),
        typeof(object),
        typeof(AbstractBaseUpDown<T>),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets/Sets a Command Parameter for the Command <see cref="Command"/> binding
    /// that can be invoked when a up/down button is clicked.
    /// </summary>
    public object CommandParameter
    {
        get => (object)GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Identifies the InputElement Dependency Property.
    /// </summary>
    public static readonly DependencyProperty InputElementProperty = DependencyProperty.Register(
        nameof(CommandTarget),
        typeof(UIElement),
        typeof(AbstractBaseUpDown<T>),
        null);

    /// <summary>
    /// Gets or sets the InputElement assigned to the control.
    /// </summary>
    public UIElement CommandTarget
    {
        get => (UIElement)GetValue(InputElementProperty);
        set => SetValue(InputElementProperty, value);
    }

    #region CommandHelper

    /// <summary>
    /// Executes a bound click command that is invoked when a up/down button is clicked
    /// (supporting <see cref="RoutedCommand"/> and <see cref="ICommand"/> bindings)
    /// </summary>
    /// <param name="cmd"></param>
    private void CommandExecute(ICommand cmd)
    {
        if (cmd is XamlUICommand command)
        {
            command.Execute(CommandParameter/*TODO, CommandTarget*/);
        }
        else
        {
            cmd?.Execute(CommandParameter);
        }
    }

    /// <summary>
    /// Is invocked when the command binding for a bound click command changes
    /// (Click Command is invoked when a up/down button is clicked)
    /// (supporting <see cref="RoutedCommand"/> and <see cref="ICommand"/> bindings).
    /// </summary>
    /// <param name="oldCommand"></param>
    /// <param name="newCommand"></param>
    private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
    {
        if (oldCommand != null)
        {
            oldCommand.CanExecuteChanged -= CanExecuteChanged;
        }
        if (newCommand != null)
        {
            newCommand.CanExecuteChanged += CanExecuteChanged;
        }
    }

    /// <summary>
    /// Determines whether a bound click command (that is invoked when a up/down button is clicked)
    /// can currently execute, or not, based on the currently bound 'CanExecute' method.
    /// (supporting <see cref="RoutedCommand"/> and <see cref="ICommand"/> bindings)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CanExecuteChanged(object sender, EventArgs e)
    {
        if (Command is XamlUICommand command)
        {
            IsEnabled = command.CanExecute(CommandParameter/*, CommandTarget*/);
        }
        else if (Command != null)
        {
            IsEnabled = Command.CanExecute(CommandParameter);
        }
    }

    #endregion

    #endregion

    #region properties

    /// <summary>
    /// Gets or sets the value assigned to the control.
    /// </summary>
    public T Value
    {
        get => (T)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Get/set dependency property to define the minimum legal value.
    /// </summary>
    public T MinValue
    {
        get => (T)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Get/set dependency property to define the maximum legal value.
    /// </summary>
    public T MaxValue
    {
        get => (T)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Implements an abstract place holder for a dependency property that should
    /// be implemented in a deriving class. The place holder is necessary here because
    /// the default value (usually 1 or greater 0) cannot be formulated with {T}.
    ///
    /// Gets or sets the step size (actual distance) of increment or decrement step.
    /// This value should at least be 1 or greater.
    /// </summary>
    public abstract T SmallChange
    {
        get; set;
    }

    /// <summary>
    /// Implements an abstract place holder for a dependency property that should
    /// be implemented in a deriving class. The place holder is necessary here because
    /// the default value (usually greater than 1) cannot be formulated with {T}.
    ///
    /// Gets or sets a large step size (actual distance) of increment or decrement step.
    /// This value should be greater than 1 but at least 1.
    /// </summary>
    public abstract T LargeChange
    {
        get; set;
    }

    /// <summary>
    /// Gets/sets the number of characters to display in the textbox portion of the
    /// AbstractBaseUpDown control.
    /// </summary>
    public int DisplayLength
    {
        get => (int)GetValue(DisplayLengthProperty);
        set => SetValue(DisplayLengthProperty, (int)value);
    }

    /// <summary>
    /// Gets/sets the MinWidth for the control. The width of the textbox portion of
    /// the control is expanded to fill the MinWidth value while the width of the
    /// UpDown buttons are auto sized.
    /// </summary>
    public virtual double MinWidth
    {
        get => (double)GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    public bool IsDeleteButtonVisible
    {
        get => (bool)GetValue(IsDeleteButtonVisibleProperty);
        set => SetValue(IsDeleteButtonVisibleProperty, value);
    }

    /// <summary>
    /// Gets/sets whether the textbox portion of the numeric up down control
    /// can go grow and shrink with its input or whether it should stay with
    /// a fixed width.
    /// </summary>
    public bool IsDisplayLengthFixed
    {
        get => (bool)GetValue(IsDisplayLengthFixedProperty);
        set => SetValue(IsDisplayLengthFixedProperty, value);
    }

    /// <summary>
    /// Gets/sets a .Net FormatString that is applied to the textbox text
    /// portion of the up down control.
    /// </summary>
    public string FormatString
    {
        get
        {
            var fsp = (string)GetValue(FormatStringProperty);
            if (fsp == "G" && (NumberStyle == System.Globalization.NumberStyles.HexNumber) ||
                (NumberStyle == System.Globalization.NumberStyles.AllowHexSpecifier))
            {
                fsp = "X";
            }
            return fsp;
        }
        set => SetValue(FormatStringProperty, value);
    }

    /// <summary>
    /// Gets/sets a dependency property to determine whether all text
    /// in the textbox should be selected on textbox focus or not.
    /// </summary>
    public bool SelectAllTextOnFocus
    {
        get => (bool)GetValue(SelectAllTextOnFocusProperty);
        set => SetValue(SelectAllTextOnFocusProperty, value);
    }

    /// <summary>
    /// Gets/sets the accelerator key of type <see cref="ModifierKeys"/> that can be pressed
    /// on the keyboard during mouse wheel scrolling over the control. Pressing the mousewheel
    /// accelerator key results in using <see cref="LargeChange"/> as base of increment/decrement
    /// steps, while otherwise the <see cref="SmallChange"/> property is applied as base of
    /// increments/decrement steps.
    /// </summary>
    public VirtualKeyModifiers MouseWheelAccelaratorKey
    {
        get => (VirtualKeyModifiers)GetValue(MouseWheelAccelaratorKeyProperty);
        set => SetValue(MouseWheelAccelaratorKeyProperty, value);
    }

    /// <summary>
    /// Gets/sets whether the mouse can be used to increment/decrement the displayed value
    /// be dragging the mouse over the control.
    ///
    /// https://github.com/Dirkster99/NumericUpDownLib/issues/2
    /// </summary>
    public bool IsMouseDragEnabled
    {
        get => (bool)GetValue(IsMouseDragEnabledProperty);
        set => SetValue(IsMouseDragEnabledProperty, value);
    }

    /// <summary>
    /// Gets/sets wether small/large step sizes can be incremented/decremented
    /// both with vertical/horizontal mouse drag moves or,
    /// whether only horizontal or only vertical mouse drag moves can
    /// incremented/decremented only in small or only in large values.
    /// </summary>
    public CanIncDecMouseDrag CanMouseDrag
    {
        get => (CanIncDecMouseDrag)GetValue(CanMouseDragProperty);
        set => SetValue(CanMouseDragProperty, value);
    }

    public bool MouseWheelEnabled
    {
        get => (bool)GetValue(MouseWheelEnabledProperty);
        set => SetValue(MouseWheelEnabledProperty, value);
    }

    /// <summary>
    /// Gets/sets wether enable large step Increment/Decrement
    /// </summary>
    public bool IsLargeChangeEnabled
    {
        get => (bool)GetValue(IsLargeStepEnabledProperty);
        set => SetValue(IsLargeStepEnabledProperty, value);
    }

    /// <summary>
    /// Gets/sets wether enable updata Value when Lost Focus
    /// </summary>
    public bool IsUpdateValueWhenLostFocus
    {
        get => (bool)GetValue(IsUpdateValueWhenLostFocusProperty);
        set => SetValue(IsUpdateValueWhenLostFocusProperty, value);
    }

    /// <summary>
    /// Gets/sets wether enable updata Value when Lost Focus
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(ViewOrientationProperty);
        set => SetValue(ViewOrientationProperty, value);
    }

    private bool _IsDataValid = true;

    /// <summary>
    /// Gets/sets determines the input text is valid or not.
    /// </summary>
    public bool IsValueValid
    {
        get => _IsDataValid;

        protected set
        {
            if (_IsDataValid != value)
            {
                _IsDataValid = value;

                EditingColorBrush = _IsDataValid ? new SolidColorBrush(Colors.Green) :
                    new SolidColorBrush(Colors.Red);

                // FIX THE behavior when user input unsupported char like ghijk
                if (EnableValidatingIndicator && !_IsDataValid)
                {
                    EditingVisibility = Visibility.Visible;
                }
            }
        }
    }

    private T lastEditingNumericValue;

    /// <summary>
    /// Gets/sets the newest available value while inputting for data verification
    /// </summary>
    public T LastEditingNumericValue
    {
        get => lastEditingNumericValue;
        protected set
        {
            lastEditingNumericValue = value;
            if (EnableValidatingIndicator)
                EditingVisibility = lastEditingNumericValue.Equals(Value) ? Visibility.Collapsed : Visibility.Visible;
            else
                EditingVisibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// Determines whether last text input was from a user (key was down) or not.
    /// </summary>
    protected bool UserInput
    {
        get; set;
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Increments the current value by the <paramref name="stepValue"/> and returns
    /// true if maximum allowed value was not reached, yet. Or returns false and
    /// changes nothing if maximum value is equal current value.
    /// </summary>
    /// <param name="stepValue"></param>
    /// <returns></returns>
    abstract protected bool OnIncrement(T stepValue);

    /// <summary>
    /// Decrements the current value by the <paramref name="stepValue"/> and returns
    /// true if minimum allowed value was not reached, yet. Or returns false and
    /// changes nothing if minimum value is equal current value.
    /// </summary>
    /// <param name="stepValue"></param>
    /// <returns></returns>
    abstract protected bool OnDecrement(T stepValue);


    private void _PART_TextBox_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
    {
        //throw new NotImplementedException();
    }

    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {

        base.OnPointerWheelChanged(e);

        // 获取指针属性
        Microsoft.UI.Input.PointerPointProperties properties = e.GetCurrentPoint(this).Properties;

        var pointer = e.Pointer;

        if (!MouseWheelEnabled)
            return;
        if (e.Handled == false)
        {
            var Delta = properties.MouseWheelDelta;
            if (Delta != 0)
            {
                if (Delta < 0 && CanDecreaseCommand() == true)
                {
                    //if (VirtualKeyModifiers== this.MouseWheelAccelaratorKey)
                    //{
                    //    if (IsLargeStepEnabled)
                    //        OnDecrement(LargeStepSize);
                    //}
                    //else
                    OnDecrease();

                    e.Handled = true;
                }
                else
                {
                    if (Delta > 0 && CanIncreaseCommand() == true)
                    {
                        //if (System.Windows.Input.Keyboard.Modifiers == this.MouseWheelAccelaratorKey)
                        //{
                        //    if (IsLargeStepEnabled)
                        //        OnIncrement(LargeStepSize);
                        //}
                        //else
                        OnIncrease();

                        e.Handled = true;
                    }
                }
            }
        }
        //if (pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
        //{
        //    //var pointer = e.GetCurrentPoint((UIElement)sender);
        //    if (properties.IsHorizontalMouseWheel)
        //    {
        //        // 如果是水平滚轮，则获取水平 delta 值
        //        var horizontalDelta = properties.MouseWheelDelta;
        //    }
        //    else
        //    {
        //        // 否则获取垂直 delta 值
        //        var verticalDelta = properties.MouseWheelDelta;
        //        Value += (T)verticalDelta;
        //    }
        //}
    }

    private void OnDeleteButtonVisibilityPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (DeleteButton is not null && IsDeleteButtonVisible is false)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
        }
    }


    #region IsMouseDragEnabled
    /// <summary>
    /// Is invoked when <see cref="IsMouseDragEnabled"/> dependency property value
    /// has been changed to update all states accordingly.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnIsMouseDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as AbstractBaseUpDown<T>).OnIsMouseDragEnabledChanged(e);
    }

    /// <summary>
    /// Is invoked when <see cref="IsMouseDragEnabled"/> dependency property value
    /// has been changed to update all states accordingly.
    /// </summary>
    /// <param name="e"></param>
    private void OnIsMouseDragEnabledChanged(DependencyPropertyChangedEventArgs e)
    {
        _objMouseIncr = null;

        if (_PART_TextBox != null)
        {
            /* TODO
             if ((bool)(e.NewValue) == false)
                _PART_TextBox.Cursor = Cursors.IBeam;
            else
                _PART_TextBox.Cursor = Cursors.ScrollAll;
            */
        }
    }
    #endregion IsMouseDragEnabled

    #region textbox mouse and focus handlers

#if WPF
    /// <summary>
    /// Clears the focus and resets the mouse incrementor object to cancel
    /// editing and return to mouse drag mode.
    ///
    /// https://www.codeproject.com/tips/478376/setting-focus-to-a-control-inside-a-usercontrol-in
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void this_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate ()
        {
            if (this.IsKeyboardFocused)
            {
                Keyboard.ClearFocus();
            }

            _objMouseIncr = null;
        }));
    }

#endif
    private void IncDecButton_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        // Remove focus when escape was hit to go back to Cursors.ScrollAll mode
        // and edit value increment/decrement via mouse drag gesture
        if (e.Key == VirtualKey.Escape)
        {
            // TODO KeyboardAccelerator.ClearFocus();
            e.Handled = true;
            return;
        }
    }

    private void _PART_TextBox_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {

    }
#if false
    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {
        base.OnPointerWheelChanged(e);


        if (!MouseWheelEnabled)
            return;
        if (e.Handled == false)
        {
            if (e.Delta != 0)
            {
                if (e.Delta < 0 && CanDecreaseCommand() == true)
                {
                    if (System.Windows.Input.Keyboard.Modifiers == this.MouseWheelAccelaratorKey)
                    {
                        if (IsLargeStepEnabled)
                            OnDecrement(LargeStepSize);
                    }
                    else
                        OnDecrease();

                    e.Handled = true;
                }
                else
                {
                    if (e.Delta > 0 && CanIncreaseCommand() == true)
                    {
                        if (System.Windows.Input.Keyboard.Modifiers == this.MouseWheelAccelaratorKey)
                        {
                            if (IsLargeStepEnabled)
                                OnIncrement(LargeStepSize);
                        }
                        else
                            OnIncrease();

                        e.Handled = true;
                    }
                }
            }
        }
    }

#endif
#if WPF
    /// <summary>
    /// This is called if we are losing the mouse capture without going through
    /// the MouseUp event - normally this should not be necessary but we'll have
    /// it as a safety net here.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_LostMouseCapture(object sender, MouseEventArgs e)
    {
        _objMouseIncr = null;
    }

    /// <summary>
    /// Is invoked if/when the user has stopped clicking the mous button
    /// over the textbox portion of the control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (IsMouseDragEnabled == false)
            return;

        if (_objMouseIncr != null && IsReadOnly == false)
        {
            var mouseUpPosition = GetPositionFromThis(e);
            if (_objMouseIncr.InitialPoint.Equals(mouseUpPosition))
            {
                _PART_TextBox.Focus();
            }
        }

        _PART_TextBox.ReleaseMouseCapture();
        _objMouseIncr = null;
    }

    private void _PART_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMouseDragEnabled == false)
            return;

        if (IsKeyboardFocusWithin == false)
        {
            _objMouseIncr = new MouseIncrementor(this.GetPositionFromThis(e), MouseDirections.None);
            e.Handled = true;
        }
    }

    private void _PART_TextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (IsMouseDragEnabled == false)
            return;

        // nothing to do here
        if (_objMouseIncr == null)
            return;

        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        if (CanIncreaseCommand() == false && CanDecreaseCommand() == false)
        {
            // since we can't parse the value, we are out of here, i.e. user put text in our number box
            _objMouseIncr = null;
            return;
        }

        var pos = GetPositionFromThis(e);
        double deltaX = (CanMouseDrag == CanIncDecMouseDrag.VerticalOnly ? 0 : _objMouseIncr.Point.X - pos.X);
        double deltaY = (CanMouseDrag == CanIncDecMouseDrag.HorizontalOnly ? 0 : _objMouseIncr.Point.Y - pos.Y);

        if (_objMouseIncr.MouseDirection == MouseDirections.None)
        {
            // this is our first time here, so we need to record if we are tracking x or y movements
            if (_objMouseIncr.SetMouseDirection(pos) != MouseDirections.None)
                _PART_TextBox.CaptureMouse();
        }

        if (_objMouseIncr.MouseDirection == MouseDirections.LeftRight)
        {
            if (IsLargeStepEnabled)
            {
                if (deltaX > 0)
                    OnDecrement(LargeStepSize);
                else
                {
                    if (deltaX < 0)
                        OnIncrement(LargeStepSize);
                }
            }
        }
        else
        {
            if (_objMouseIncr.MouseDirection == MouseDirections.UpDown)
            {
                if (deltaY > 0)
                {
                    if (CanIncreaseCommand() == true)
                        OnIncrease();
                }
                else
                {
                    if (deltaY < 0)
                    {
                        if (CanDecreaseCommand() == true)
                            OnDecrease();
                    }
                }
            }
        }

        _objMouseIncr.Point = GetPositionFromThis(e);
    }

    private Point GetPositionFromThis(MouseEventArgs e)
    {
        return this.PointToScreen(e.GetPosition(this));
    }

    /// <summary>
    /// Go back to showing <see cref="Cursors.ScrollAll"/> mouse cursor on mouse over
    /// without keyboard focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (IsMouseDragEnabled == false)
            return;

        _objMouseIncr = null;
        (sender as TextBox).Cursor = Cursors.ScrollAll;
    }
#endif

    private static void DisplayLengthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
#if false
        if (obj is AbstractBaseUpDown<T> control && control._PART_TextBox != null)
        {
            control._PART_TextBox.MinWidth = control._PART_Measuring_Element.ActualWidth;
            control._PART_TextBox.Width = control._PART_Measuring_Element.ActualWidth;
            control._PART_TextBox.UpdateLayout();
        }
#endif
    }


    private void _PART_Measuring_Element_LayoutUpdated(object sender, object e)
    {
        _PART_TextBox.MinWidth = _PART_Measuring_Element.ActualWidth;
        _PART_TextBox.Width = _PART_Measuring_Element.ActualWidth;
        _PART_TextBox.MaxWidth = _PART_Measuring_Element.ActualWidth;
        _PART_TextBox.UpdateLayout();
        //Debug.WriteLine(_PART_Measuring_Element.Shadow);
    }

    private void _PART_Measuring_Element_TextChanged(object sender, TextChangedEventArgs e)
    {
        //_PART_TextBox.MinWidth = _PART_Measuring_Element.ActualWidth;
        //_PART_TextBox.Width = _PART_Measuring_Element.ActualWidth;
        //_PART_TextBox.UpdateLayout();
    }

    private void _PART_TextBox_SelectionChanging(TextBox sender, TextBoxSelectionChangingEventArgs args)
    {
        //todo throw new NotImplementedException();
    }

    private void _PART_TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        //todo throw new NotImplementedException();
    }

    /// <summary>
    /// Adjust mouse cursor to <see cref="Cursors.ScrollAll"/> when mouse
    /// hovers over the <see cref="TextBox"/> without keyboard focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_MouseEnter(object sender, PointerRoutedEventArgs e)
    {
        if (IsMouseDragEnabled == false)
            return;

        /* TODO if (IsKeyboardFocusWithin)
            (sender as TextBox).Cursor = Cursors.IBeam;
        else
            (sender as TextBox).Cursor = Cursors.ScrollAll;*/
    }


#if WPF

    /// <summary>
    /// Force <see cref="Cursors.IBeam"/> cursor when keyboard focus is within control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        _objMouseIncr = null;
        (sender as TextBox).Cursor = Cursors.IBeam;
    }
#endif
    private void _PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        var tb = sender as TextBox;
        LastfocusState = tb.FocusState;
        lastDisplayLens = (byte)DisplayLength;
        if (tb.FocusState == FocusState.Keyboard && IsDeleteButtonVisible)
        {
            spMode = SpinButtonPlacementMode;
            doubleActualWidth = _PART_IncDecStackPanel.Visibility == Visibility.Visible ? _PART_IncDecStackPanel.ActualWidth : 0;
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden;

            //_PART_TextBox.Width = _PART_TextBox.ActualWidth + doubleActualWidth;
            DisplayLength += (spMode == NumberBoxSpinButtonPlacementMode.Inline ? 6 : 3);
        }

        Debug.WriteLine(tb.FocusState.ToString());

        _objMouseIncr = null;
        if (SelectAllTextOnFocus == true)
        {
            if (tb != null)
                tb.SelectAll();
        }
    }

    NumberBoxSpinButtonPlacementMode spMode = NumberBoxSpinButtonPlacementMode.Compact;
    double doubleActualWidth = 0;

    FocusState LastfocusState = FocusState.Pointer;
    byte lastDisplayLens = 1;
    private void _PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        UserInput = false;
        var tb = sender as TextBox;
        SpinButtonPlacementMode = spMode;
        if (LastfocusState == FocusState.Keyboard)
        {
            //_PART_TextBox.Width = _PART_TextBox.ActualWidth - doubleActualWidth;
            DisplayLength = (lastDisplayLens);
        }


        Debug.WriteLine(tb.FocusState.ToString());
        if (IsMouseDragEnabled == true)
        {
            _objMouseIncr = null;
            // TODO (sender as TextBox).Cursor = Cursors.ScrollAll;
        }


        // format the value string if value is no changed
        if (Value.Equals(LastEditingNumericValue))
            Value = FormatText(_PART_TextBox.Text);


        // trigger the change event if IsUpdateValueWhenLostFocus=true and value is valid
        if (IsUpdateValueWhenLostFocus && IsValueValid)
        {
            Value = FormatText(_PART_TextBox.Text, true);
        }

    }
    #endregion textbox mouse and focus handlers

    #region textinput handlers

    private void _PART_TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Method executes when the text portion in the textbox is changed
    /// The Value is corrected to a valid value if text was illegal or
    /// value was outside of the specified bounds.
    ///
    /// https://stackoverflow.com/questions/841293/where-is-the-wpf-numeric-updown-control#2752538
    /// also, <see cref="_PART_TextBox_PreviewKeyDown"/> Text will be format by "Enter"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void _PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        T temp = LastEditingNumericValue;
        if (UserInput == true)
        {
            IsValueValid = VerifyText(_PART_TextBox.Text, ref temp);
            if (!LastEditingNumericValue.Equals(temp))
            {
                LastEditingNumericValue = temp;
            }
#if false
                int pos = _PART_TextBox.CaretIndex;

                FormatText(_PART_TextBox.Text, false);

                if (_PART_TextBox.IsFocused == false)
                    UserInput = false;

                _PART_TextBox.CaretIndex = pos;
#endif
        }
        else
        {
            IsValueValid = VerifyText(_PART_TextBox.Text, ref temp);
        }

    }

    /// <summary>
    /// Catches pasting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_Paste(object sender, TextControlPasteEventArgs e)
    {

        TextBox textBox = sender as TextBox;
        var package = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
        if (!package.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
        {
            var text = package.GetTextAsync();
            return;
        }

        UserInput = true;
    }

    /// <summary>
    /// Catches Backspace, Delete, Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    // TODO private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    //{
    //    UserInput = true;
    //}

    /// <summary>
    /// Catches pasting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _PART_TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        UserInput = true;
        var SelectionStart = _PART_TextBox.SelectionStart;

        // Remove focus when escape was hit to go back to Cursors.ScrollAll mode
        // and edit value increment/decrement via mouse drag gesture
        if (e.Key == VirtualKey.Escape)
        {
            // SUPPORT RESUME TO THE LAST VALUE WHEN USER DECIDE TO Exit editing

            _PART_TextBox.Text = FormatNumber(Value);
            _PART_TextBox.SelectionStart = SelectionStart;
            // Keyboard.ClearFocus();
            e.Handled = true;
            return;
        }

        // support small value change via up cursor key
        if (e.Key == VirtualKey.Up && IsModifierKeyDown() == false)
        {
            if (CanIncreaseCommand() == true)
            {
                IncreaseCommand.Execute(this);
            }

            e.Handled = true;
            return;
        }

        // support small value change via down cursor key
        if (e.Key == VirtualKey.Down && IsModifierKeyDown() == false)
        {
            if (CanDecreaseCommand() == true)
                DecreaseCommand.Execute(this);

            e.Handled = true;
            return;
        }

        if (IsLargeChangeEnabled)
        {
            // support large value change via right cursor key
            if (e.Key == VirtualKey.Right && IsModifierKeyDown() == false)
            {
                OnIncrement(LargeChange);
                e.Handled = true;
                return;
            }

            // support large value change via left cursor key
            if (e.Key == VirtualKey.Left && IsModifierKeyDown() == false)
            {
                OnDecrement(LargeChange);
                e.Handled = true;
                return;
            }
        }
#if false
        var window = CoreWindow.GetForCurrentThread();
        var isCtrlDown = (window.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
#else
        var isCtrlDown = false;
#endif
        // update value typed by the user
        if (e.Key == VirtualKey.Enter)
        {
            if (_PART_TextBox != null)
            {
                if (!IsValueValid)
                {
                    e.Handled = true;
                    return;
                }
                T OldValue = Value;
                Value = FormatText(_PART_TextBox.Text, true);
                _PART_TextBox.SelectionStart = SelectionStart;

                // force to raise value changed event to tigger re write /re-set for application
                if (OldValue.Equals(Value) && isCtrlDown)
                {
                    System.Diagnostics.Debug.WriteLine("ValueChanged forced by user");
                    this.OnValueChanged(new ValueChangedEventArgs<T>(Value, Value));
                }
                LastEditingNumericValue = Value;
                e.Handled = true;
            }
            return;
        }
    }

    /// <summary>
    /// Gets whether any keyboard modifier (ALT, SHIFT, or CTRL) is down or not.
    /// </summary>
    /// <returns></returns>
    private bool IsModifierKeyDown()
    {
        CoreWindow window = CoreWindow.GetForCurrentThread();
        if (window == null) return false;

        var csaDown = false;

        if ((window.GetKeyState(VirtualKey.Shift) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
        {
            // Shift 键正在按下
            csaDown |= true;
        }

        if ((window.GetKeyState(VirtualKey.Control) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
        {
            // Ctrl 键正在按下
            csaDown |= true;
        }

        if ((window.GetKeyState(VirtualKey.Menu) & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down)
        {
            // Alt 键正在按下
            csaDown |= true;
        }
        return csaDown;
    }

    #endregion

    #region string format and value changed

    /// <summary>
    /// Gets a formatted string for the value of the number passed in
    /// and ensures that a default string is returned even if there is
    /// no format specified.
    /// </summary>
    /// <param name="number">.Net type specific value to be formated as string</param>
    /// <returns>The string that was formatted with the FormatString
    /// dependency property</returns>
    protected string FormatNumber(T number)
    {
        var format = "{0}";
        var form = (string)GetValue(FormatStringProperty);
        if (string.IsNullOrEmpty(this.FormatString) == false)
        {
            format = !FormatString.StartsWith("{")
                ? "{0:" + this.FormatString + "}"
                : FormatString;
        }

        return string.Format(format, number);
    }

    /// <summary>
    /// Checks if the current string entered in the textbox is:
    /// 1) A valid number (syntax)
    /// 2) within bounds (Min &lt;= number &lt;= Max )
    ///
    /// 3) adjusts the string if it appears to be invalid and
    ///
    /// 4) <paramref name="formatNumber"/> true:
    ///    Applies the FormatString property to format the text in a certain way
    /// </summary>
    /// <param name="text"></param>
    /// <param name="formatNumber"></param>
    /// <returns>the value of the string with special format</returns>
    protected T FormatText(string text, bool formatNumber = true)
    {
        if (_PART_TextBox == null)
            return Value;

        var SelectionStart = _PART_TextBox.SelectionStart;

        T number = default;
        // Does this text represent a valid number ?
        if (ParseText(text, out number))
        {
            number = CoerceValue(number);

            _PART_TextBox.Text = FormatNumber(number);
            _PART_TextBox.SelectionStart = SelectionStart;

            return number;
        }

        // Reset to last value since string does not appear to represent a number
        _PART_TextBox.Text = FormatNumber(Value);
        _PART_TextBox.SelectionStart = SelectionStart;
        return LastEditingNumericValue;
    }

    protected abstract bool ParseText(string text, out T number);

    /// <summary>
    /// Verify the text is valid or not while use is typing
    /// </summary>
    /// <param name="text"></param>
    protected abstract bool VerifyText(string text, ref T tempValue);


    #endregion textinput handlers

    #region Coerce Value MinValue MaxValue abstract methods
    /// <summary>
    /// Attempts to force the new value into the existing dependency property
    /// and attempts backup plans (uses minimum or maximum values) if value appears
    /// to be out of either range.
    /// </summary>
    /// <param name="NewValue"></param>
    /// <returns></returns>
    protected abstract T CoerceValue(T NewValue);

    /// <summary>
    /// Attempts to force the new Minimum value into the existing dependency property
    /// and attempts backup plans (uses minimum or maximum values) if value appears
    /// to be out of either range.
    /// </summary>
    /// <param name="NewValue"></param>
    /// <returns></returns>
    protected abstract T CoerceMinValue(T NewValue);

    /// <summary>
    /// Attempts to force the new Minimum value into the existing dependency property
    /// and attempts backup plans (uses maximum or maximum values) if value appears
    /// to be out of either range.
    /// </summary>
    /// <param name="NewValue"></param>
    /// <returns></returns>
    protected abstract T CoerceMaxValue(T NewValue);
    #endregion  Coerce Value MinValue MaxValue abstract methods

    #region Value dependency property helper methods
    /// <summary>
    /// CHANGE the display of value;
    /// <see cref="InputBaseUpDown.NumberStyle"/>: before change the formatstring, please set Numberstyle is match the FormatString
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    private static void OnFormatStringChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is AbstractBaseUpDown<T> control && control._PART_TextBox != null && e.NewValue is string)
        {
            var SelectionStart = control._PART_TextBox.SelectionStart;

            control._PART_TextBox.Text = control.FormatNumber(control.Value);
            control._PART_TextBox.SelectionStart = SelectionStart;
        }
    }


    private static void OnValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var control = obj as AbstractBaseUpDown<T>;

        if (control != null && args != null)
        {
            ValueChangedEventArgs<T> e = new((T)args.OldValue, (T)args.NewValue);
            AbstractBaseUpDown<T>.CoerceValue(obj, args.NewValue);
            control.OnValueChanged(e);
        }
    }


    private static object CoerceValue(DependencyObject element, object value)
    {
        var control = element as AbstractBaseUpDown<T>;

        try
        {
            T newValue = (T)value;

            if (control != null)
                return control.CoerceValue(newValue);
        }
        catch
        {
        }

        return control.Value;
    }

    /// <summary>
    /// Raises the ValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnValueChanged(ValueChangedEventArgs<T> args)
    {
        if (_PART_TextBox != null)
        {
            var SelectionStart = _PART_TextBox.SelectionStart;
            _PART_TextBox.Text = FormatNumber(Value);
            _PART_TextBox.SelectionStart = SelectionStart;

            LastEditingNumericValue = Value;
        }
        CommandExecute(Command);
        ValueChanged?.Invoke(this, args);
    }


    #endregion Value dependency property helper methods

    #region MinValue dependency property helper methods
    private static void OnMinValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        var control = d as AbstractBaseUpDown<T>;

        if (control != null && args != null)
        {
            MinValueChangedEventArgs<T> e = new((T)args.OldValue, (T)args.NewValue);
            control.OnMinValueChanged(e);

            AbstractBaseUpDown<T>.CoerceMinValue(d, args.NewValue);
        }
    }

    private static object CoerceMinValue(DependencyObject element, object value)
    {
        var control = element as AbstractBaseUpDown<T>;

        try
        {
            T newValue = (T)value;

            if (control != null)
            {
                return control.CoerceMinValue(newValue);
            }
        }
        catch
        {
        }

        return control.MinValue;
    }

    /// <summary>
    /// Raises the MinValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnMinValueChanged(MinValueChangedEventArgs<T> args)
    {
        // TODO MinValueChangeds.InvocationList?.Invoke(this, args);
    }

    #endregion Value dependency property helper methods

    #region MaxValue dependency property helper methods

    private static void OnMaxValuePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var control = obj as AbstractBaseUpDown<T>;

        if (control != null && args != null)
        {
            MaxValueChangedEventArgs<T> e = new MaxValueChangedEventArgs<T>((T)args.OldValue, (T)args.NewValue/*, MaxValueChangedEvent*/);
            control.OnMaxValueChanged(e);

            AbstractBaseUpDown<T>.CoerceMaxValue(obj, args.NewValue);
        }
    }

    private static object CoerceMaxValue(DependencyObject element, object value)
    {
        var control = element as AbstractBaseUpDown<T>;

        try
        {
            T newValue = (T)value;

            if (control != null)
                return control.CoerceMaxValue(newValue);

            return newValue;
        }
        catch (Exception)
        {
        }

        return control.MaxValue;
    }

    /// <summary>
    /// Raises the MinValueChanged event.
    /// </summary>
    /// <param name="args">Arguments associated with the ValueChanged event.</param>
    protected virtual void OnMaxValueChanged(MaxValueChangedEventArgs<T> args)
    {
        // TODO MaxValueChangeds.InvocationList?.Invoke(this, args);
    }

    #endregion Value dependency property helper methods

    #region DisplayLength IsDisplayLengthFixed
    /// <summary>
    /// Sets or unsets the binding between measuring and user textbox.
    /// </summary>
    /// <param name="SetBinding"></param>
    private void BindMeasuringObject(bool SetBinding = true)
    {
        BindMeasuringObject(_PART_TextBox, _PART_Measuring_Element, SetBinding);
    }

    /// <summary>
    /// Sets or Unsets a binding between a
    /// - MeasuringControl.ActualWidth and
    /// - UserControl.MaxWidth
    ///
    /// Both controls can be any <see cref="FrameworkElement"/>.
    /// </summary>
    /// <param name="UserControl"></param>
    /// <param name="MeasuringControl"></param>
    /// <param name="SetBinding"></param>
    private void BindMeasuringObject(FrameworkElement UserControl,
                                     FrameworkElement MeasuringControl,
                                     bool SetBinding = true)
    {
        if (UserControl != null)
        {
            UserControl.ClearValue(FrameworkElement.MaxWidthProperty);

            if (SetBinding == true && MeasuringControl != null)
            {
                Binding binding = new Binding
                {
                    Path = new PropertyPath("ActualWidth"),
                    Source = MeasuringControl
                };

                BindingOperations.SetBinding(UserControl, FrameworkElement.MaxWidthProperty, binding);
            }
        }
    }

    /// <summary>
    /// Method is invoked when the value of the <see cref="IsDisplayLengthFixed"/>
    /// dependency property is changed. This results in changing the behavior of
    /// the textbox resizing which in turn is dependent on the binding between
    /// the PART_TextBox.NaxWidth = PART_Measuring_TextBox.ActualWidh.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnIsDisplayLengthFixedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as AbstractBaseUpDown<T>;

        if (control != null && e.NewValue is bool)
            control.BindMeasuringObject((bool)e.NewValue);
    }
    #endregion DisplayLength IsDisplayLengthFixed

    #endregion methods DisplayLength IsDisplayLengthFixed
}
