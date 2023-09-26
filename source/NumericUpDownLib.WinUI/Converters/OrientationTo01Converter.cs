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

    public sealed class OrientationTo01Converter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public OrientationTo01Converter()
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
            if (value is not Orientation && !(value is int))
                return null;
            return ((Orientation)value == Orientation.Horizontal) ? 0 : 1;
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
                return 1;

            if (Equals(value, Orientation.Horizontal))
                return 0;
            return null;
        }
        #endregion methods
    }
}
