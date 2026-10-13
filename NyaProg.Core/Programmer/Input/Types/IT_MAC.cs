using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extension.Network;

namespace Programmer.Input.Types
{
    public class IT_MAC : InputType
    {
        public IT_MAC() : base("MAC", true)
        {

        }

        public override bool Check(string Value)
        {
            MACAddress val;
            return MACAddress.TryParse(Value, out val);
        }

        public override string Increment(string Value)
        {
            var val = MACAddress.Parse(Value);
            val.Increment();
            return val.ToString();
        }

        public override string Convert(string Value)
        {
            var val = MACAddress.Parse(Value);
            return val.ToString();
        }
    }
}
