using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Programmer.Input.Types;

namespace Programmer.Input
{
    public class InputTypeLibrary
    {
        private List<InputType> Types = new List<InputType>();

        public InputTypeLibrary()
        {
            // Internal types
            Types.Add(new IT_MAC());      // "MAC"
            Types.Add(new IT_MAC_old());  // "mac"
            Types.Add(new IT_IP());       // "ip"

            // Generic types
            Types.Add(new IT_Generic("str",
                s => true,
                s => s,
                null));
            Types.Add(new IT_Generic("int",
                s => { int val; return Int32.TryParse(s, out val); },
                s => s,
                s => (Int32.Parse(s) + 1).ToString()));
            Types.Add(new IT_Generic("int64",
                s => { Int64 val; return Int64.TryParse(s, out val); },
                s => s,
                s => (Int64.Parse(s) + 1).ToString()));
            Types.Add(new IT_Generic("int48",
                s => { Int64 val; return Int64.TryParse(s, out val); },
                s => (Int64.Parse(s) & 0xFFFFFFFFFFFFL).ToString(),
                s => ((Int64.Parse(s) + 1) & 0xFFFFFFFFFFFFL).ToString()));
            Types.Add(new IT_Generic("int32",
                s => { Int32 val; return Int32.TryParse(s, out val); },
                s => (Int32.Parse(s) & 0xFFFFFFFFL).ToString(),
                s => ((Int32.Parse(s) + 1) & 0xFFFFFFFFL).ToString()));
            Types.Add(new IT_Generic("hex32",
                s => { UInt32 val; return UInt32.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => (UInt32.Parse(s, NumberStyles.HexNumber) & 0xFFFFFFFFL).ToString("X8"),
                s => ((UInt32.Parse(s, NumberStyles.HexNumber) + 1) & 0xFFFFFFFFL).ToString("X8")));
            Types.Add(new IT_Generic("hex48",
                s => { UInt64 val; return UInt64.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => (UInt64.Parse(s, NumberStyles.HexNumber) & 0xFFFFFFFFFFFFL).ToString("X12"),
                s => ((UInt64.Parse(s, NumberStyles.HexNumber) + 1) & 0xFFFFFFFFFFFFL).ToString("X12")));
            Types.Add(new IT_Generic("hex64",
                s => { UInt64 val; return UInt64.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out val); },
                s => UInt64.Parse(s, NumberStyles.HexNumber).ToString("X16"),
                s => (UInt64.Parse(s, NumberStyles.HexNumber) + 1).ToString("X16")));
        }

        /// <summary>
        /// Получить обработчик типа по его названию
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public InputType GetInputType(string Type)
        {
            foreach (var T in Types)
            {
                if (T.Type.CompareTo(Type) == 0)
                    return T;
            }

            return null;
        }
    }
}
