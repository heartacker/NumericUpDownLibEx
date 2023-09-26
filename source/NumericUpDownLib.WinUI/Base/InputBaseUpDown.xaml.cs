// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NumericUpDownLib.WinUI.Base;

public abstract partial class InputBaseUpDown : Control
{
    public InputBaseUpDown()
    {
        this.DefaultStyleKey = typeof(InputBaseUpDown);
        InitializeCommands();
    }

    #region fields

    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(InputBaseUpDown),
        new PropertyMetadata(false));

    /// <summary>
    /// Determines the allowed style of a number entered and displayed in the textbox.
    /// </summary>
    public static readonly DependencyProperty NumberStyleProperty = DependencyProperty.Register(
        nameof(NumberStyle),
        typeof(NumberStyles),
        typeof(InputBaseUpDown),
        new PropertyMetadata(NumberStyles.Any));

    /// <summary>
    /// Backing store of <see cref="EnableValidatingIndicator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EnableValidatingIndicatorProperty = DependencyProperty.Register(
        nameof(EnableValidatingIndicator),
        typeof(bool),
        typeof(InputBaseUpDown),
        new PropertyMetadata(true));

    /// <summary>
    /// Backing store of <see cref="EditingVisibility"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditingVisibilityProperty = DependencyProperty.Register(
        nameof(EditingVisibility),
        typeof(Visibility),
        typeof(InputBaseUpDown),
        new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Backing store of <see cref="EditingColorBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EditingColorBrushProperty = DependencyProperty.Register(
        nameof(EditingColorBrush),
        typeof(SolidColorBrush),
        typeof(InputBaseUpDown),
        new PropertyMetadata(new SolidColorBrush(Colors.Green)));


    /// <summary>
    /// Backing store of <see cref="SpinButtonPlacementMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SpinButtonPlacementModeProperty = DependencyProperty.Register(
        nameof(SpinButtonPlacementMode),
        typeof(NumberBoxSpinButtonPlacementMode),
        typeof(InputBaseUpDown),
        new PropertyMetadata(NumberBoxSpinButtonPlacementMode.Compact,
            new PropertyChangedCallback(SpinButtonPlacementModeChanged)));


    /// <summary>
    /// Backing store of <see cref="SpinButtonPlacementMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(InputBaseUpDown),
        new PropertyMetadata(""));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Backing store for dependency property for .Net FormatString that is
    /// applied to the textbox text portion of the up down control.
    /// </summary>
    protected static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText),
        typeof(string),
        typeof(InputBaseUpDown),
        new PropertyMetadata(""));

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }


#if false
    /// <summary>
    /// Dependency property backing store for the <see cref="IsIncDecButtonsVisible"/> property.
    /// </summary>
    public static readonly DependencyProperty IsIncDecButtonsVisibleProperty = DependencyProperty.Register(
        nameof(IsIncDecButtonsVisible),
        typeof(bool),
        typeof(InputBaseUpDown),
        new PropertyMetadata(true));

    /// <summary>
    /// Gets/sets whether the Increment or Decrement button is currently visible or not.
    /// </summary>
    public bool IsIncDecButtonsVisible
    {
        get { return (bool)GetValue(IsIncDecButtonsVisibleProperty); }
        set { SetValue(IsIncDecButtonsVisibleProperty, value); }
    }
#endif

    /// <summary>
    /// identify that the inputing data is valid or not.,
    /// </summary>
    /// <value></value>
    internal SolidColorBrush EditingColorBrush
    {
        get => (SolidColorBrush)GetValue(EditingColorBrushProperty);
        set => SetValue(EditingColorBrushProperty, value);
    }


    /// <summary>
    /// identify that the editing Visibility
    /// </summary>
    /// <value></value>
    internal Visibility EditingVisibility
    {
        get => (Visibility)GetValue(EditingVisibilityProperty);
        set => SetValue(EditingVisibilityProperty, value);
    }

    /// <summary>
    /// identify that the is enable the red/green tip while editing
    /// </summary>
    /// <value></value>
    public bool EnableValidatingIndicator
    {
        get => (bool)GetValue(EnableValidatingIndicatorProperty);
        set => SetValue(EnableValidatingIndicatorProperty, value);
    }

    public NumberBoxSpinButtonPlacementMode SpinButtonPlacementMode
    {
        get => (NumberBoxSpinButtonPlacementMode)GetValue(SpinButtonPlacementModeProperty);
        set => SetValue(SpinButtonPlacementModeProperty, value);
    }

    protected RelayCommand _IncreaseCommand;

    protected RelayCommand _DecreaseCommand;

    #endregion fields


    #region properties
    /// <summary>
    /// Expose the increase value command via <seealso cref="RoutedCommand"/> property.
    /// </summary>
    public ICommand IncreaseCommand => _IncreaseCommand;

    /// <summary>
    /// Expose the decrease value command via <seealso cref="RoutedCommand"/> property.
    /// </summary>
    public ICommand DecreaseCommand => _DecreaseCommand;

    /// <summary>
    /// Determines whether the textbox portion of the control is editable
    /// (requires additional check of bounds) or not.
    /// </summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets/sets the allowed style of a number entered and displayed in the textbox.
    /// </summary>
    public NumberStyles NumberStyle
    {
        get => (NumberStyles)GetValue(NumberStyleProperty);
        set => SetValue(NumberStyleProperty, value);
    }

    #endregion properties

    #region methods

    #region methods

    private static void SpinButtonPlacementModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {

    }

    #endregion

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
    private void InitializeCommands()
    {
        _IncreaseCommand = new RelayCommand(OnIncrease
            //, canExecute: CanIncreaseCommand
            );

        _DecreaseCommand = new RelayCommand(OnDecrease
            //, canExecute: CanDecreaseCommand
            );
    }
    #endregion

    #endregion methods
}
