namespace NumericUpDownLib.WinUI.Converters
{
    using Microsoft.UI.Xaml.Data;
    using System;
    using System.Globalization;
    using System.Windows;


    /// <summary>
    /// Scales a double value by its scale factor (eg. 100.00) up (convert)
    /// or down (convertback), or vice versa if factor is set to 0.01...
    /// </summary>
    public sealed class FactorToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public FactorToDoubleConverter()
        {
            Factor = 100.0;
        }

        /// <summary>
        /// Gets/sets the factor for multiplication and division
        /// between source (viewmodel) and target (view).
        /// </summary>
        public double Factor { get; set; }

        /// <summary>
        /// Converts a <seealso cref="Visibility"/> value
        /// into a <seealso cref="Boolean"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)  // Do multplikation with float and output float
            {
                var val = (float)value;

                return (float)(val * Factor);
            }

            //todo  if (value is double)   // Do multplikation with double and output double
            {
                var val = (double)value;

                return val * Factor;
            }
        }


        /// <summary>
        /// Converts a <seealso cref="Boolean"/> value
        /// into a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is float)  // Do division with float and output float
            {
                var val = (double)((float)value);

                return (float)(val / Factor);
            }

            // if (value is double)   // Do division with double and output double
            {
                var val = (double)value;

                return val / Factor;
            }

        }
    }
}
