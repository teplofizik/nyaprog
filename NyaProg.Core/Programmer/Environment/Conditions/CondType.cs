using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Programmer.Environment.Conditions
{
    public enum CondType
    {
        None,
        Invalid,
        HasOption,
        HasNotOption,
        OptionEquals,
        OptionNotEquals
    }
}
