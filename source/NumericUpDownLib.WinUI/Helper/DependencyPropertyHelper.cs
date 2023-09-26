using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace NumericUpDownLib.WinUI.Helper;

internal class DependencyPropertyHelper
{
    public static IEnumerable<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            if (child is T targetChild)
            {
                yield return targetChild;
            }

            foreach (T grandChild in FindChildrenOfType<T>(child))
            {
                yield return grandChild;
            }
        }
    }
}
