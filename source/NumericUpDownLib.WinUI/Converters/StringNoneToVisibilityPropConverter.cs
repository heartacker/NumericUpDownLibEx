namespace NumericUpDownLib.WinUI.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;
    using System;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Converts a boolean value into a configurable
    /// value of type <seealso cref="Visibility"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>

    public sealed class StringNoneToVisibilityPropConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public StringNoneToVisibilityPropConverter()
        {
            // set defaults
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
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
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
            return null;
        }
        #endregion methods
    }
}
