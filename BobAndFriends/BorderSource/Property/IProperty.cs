using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorderSource.Property
{
    public interface IProperty
    {
        string PropertyName { get; set; }
        T GetValue<T>();
    }
}
