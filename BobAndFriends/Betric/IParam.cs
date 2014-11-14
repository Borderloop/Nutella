using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betric
{
    public interface IParam
    {
        string Name { get; set; }
        object Value { get; set; }
    }
}
