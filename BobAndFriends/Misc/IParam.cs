using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
    public interface IParam
    {
        string Name { get; set; }
        object Value { get; set; }
    }
}
