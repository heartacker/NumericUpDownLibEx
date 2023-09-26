namespace NumericUpDownLib.WinUI.Converters
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Data;
    using System;

    /// <summary>
    /// Converts a boolean value into a configurable
    /// value of type <seealso cref="Visibility"/>.
    /// 
    /// Source: http://stackoverflow.com/questions/3128023/wpf-booleantovisibilityconverter-that-converts-to-hidden-instead-of-collapsed-wh
    /// </summary>

    public sealed class HorizontalContentAlignmentToTextAlignmentConverter : IValueConverter
    {
        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public HorizontalContentAlignmentToTextAlignmentConverter()
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
            if (!(value is HorizontalAlignment) && !(value is int))
                return null;

            HorizontalAlignment ha = (HorizontalAlignment)(value);

            TextAlignment ta = ha! switch
            {
                HorizontalAlignment.Left => TextAlignment.Left,
                HorizontalAlignment.Center => TextAlignment.Center,
                HorizontalAlignment.Right => TextAlignment.Right,
                HorizontalAlignment.Stretch => TextAlignment.Right,
                _ => TextAlignment.Right
            };
            return ta;
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
            if (Equals(value, TextAlignment.Left))
                return HorizontalAlignment.Left;

            if (Equals(value, TextAlignment.Right))
                return HorizontalAlignment.Right;

            if (Equals(value, TextAlignment.Center))
                return HorizontalAlignment.Center;

            return HorizontalAlignment.Right;
        }
        #endregion methods
    }
}
