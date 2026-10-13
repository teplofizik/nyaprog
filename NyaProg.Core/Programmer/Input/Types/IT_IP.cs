using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Programmer.Input.Types
{
    public class IT_IP : InputType
    {
        public IT_IP() : base("ip", true)
        {

        }

        public override bool Check(string Value)
        {
            IPAddress val;
            return IPAddress.TryParse(Value, out val);
        }

        public override string Increment(string Value)
        {
            var val = IPAddress.Parse(Value).GetAddressBytes();
            for (int i = 0; i < val.Length; i++)
            {
                int Index = val.Length - 1 - i;
                val[Index]++;
                if (val[Index] != 0) break;
            }
            return new IPAddress(val).ToString();
        }

        public override string Convert(string Value)
        {
            return Value;
        }
    }
}
