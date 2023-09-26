namespace NumericUpDownLib.WinUI.Converters
{
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using System;

    /// <summary>
    /// Converts a boolean value into a configurable
    /// value of type <seealso cref="Visibility"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>

    public sealed class SpinButtonPlacementModeToOrientationConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public SpinButtonPlacementModeToOrientationConverter()
        {
        }
        #endregion constructor


        #region methods
        /// <summary>
        /// Convertzs a bool value into <see cref="Visibility"/> as configured in the
        /// <see cref="TrueValue"/> and <see cref="FalseValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is NumberBoxSpinButtonPlacementMode) && !(value is int))
                return null;
            return ((NumberBoxSpinButtonPlacementMode)value == NumberBoxSpinButtonPlacementMode.Inline) ? Orientation.Horizontal : Orientation.Vertical;
        }

        /// <summary>
        /// Convertzs a <see cref="Visibility"/> value into bool as configured in the
        /// <see cref="TrueValue"/> and <see cref="FalseValue"/> properties.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Equals(value, Orientation.Vertical))
                return NumberBoxSpinButtonPlacementMode.Compact;

            if (Equals(value, Orientation.Horizontal))
                return NumberBoxSpinButtonPlacementMode.Inline;


            return null;
        }
        #endregion methods
    }
}
