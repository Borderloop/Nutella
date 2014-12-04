using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Property
{
    public class Property<T> : IProperty
    {
        private bool IsSet { get; set; }

        public Property()
        {
            IsSet = false;
        }

        public string PropertyName { get; set; }

        public T DefaultValue { get; set; }

        private T _CurrentValue;
        public T CurrentValue
        {
            get
            {
                return _CurrentValue;
            }
            set
            {
                IsSet = true;
                _CurrentValue = value;
            }
        }

        public T GetValue<T>()
        {
            if (typeof(T) != DefaultValue.GetType()) throw new InvalidCastException("Cannot cast " + typeof(T).ToString() + " to " + DefaultValue.GetType().ToString());
            return (T)(object)(IsSet ? CurrentValue : DefaultValue);
        }
    }
}
