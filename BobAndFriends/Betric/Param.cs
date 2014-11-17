using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betric
{
    public class Param<T> : IParam
    {
        public string Name {get; set;}


        public object Value
        {
            get
            {
                return (T)_Value;
            }
            set
            {
                _Value = (T)value;
            }
        }
        private T _Value {get; set;}
    }
}
