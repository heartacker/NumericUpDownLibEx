using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericUpDownLib.WinUI
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        private EventHandler valueChangedEvent;

        public ValueChangedEventArgs(T oldValue, T newValue, EventHandler valueChangedEvent = null)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
            this.valueChangedEvent = valueChangedEvent;
        }

        public T OldValue { get; set; }
        public T NewValue { get; set; }

    }

    public class MinValueChangedEventArgs<T> : ValueChangedEventArgs<T>
    {
        public MinValueChangedEventArgs(T oldValue, T newValue, EventHandler valueChangedEvent = null) : base(oldValue, newValue, valueChangedEvent)
        {
        }
    }
    public class MaxValueChangedEventArgs<T> : ValueChangedEventArgs<T>
    {
        public MaxValueChangedEventArgs(T oldValue, T newValue, EventHandler valueChangedEvent = null) : base(oldValue, newValue, valueChangedEvent)
        {
        }
    }
}
