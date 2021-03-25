using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Extension.Network
{
    public class MACAddress
    {
        public UInt64 Address = 0;

        public void SetAddress(UInt64 A)
        {
            Address = A;
        }

        static public MACAddress Parse(string s)
        {
            var A = new MACAddress();
            var Parts = s.Split(new char[] { ':' });
            if (Parts.Length != 6) return null;

            int[] P = new int[6];
            for (int i = 0; i < 6; i++)
            {
                if (!Int32.TryParse(Parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out P[i])) return null;
                if ((P[i] < 0) || (P[i] > 255)) return null;
            }

            UInt64 Temp = 0;
            for (int i = 0; i < 6; i++)
                Temp |= (Convert.ToUInt64(P[5 - i]) << 8 * i);

            A.Address = Temp;
            return A;
        }

        static public bool TryParse(string s, out MACAddress A)
        {
            A = new MACAddress();
            var Parts = s.Split(new char[] { ':' });
            if (Parts.Length != 6) return false;

            int[] P = new int[6];
            for (int i = 0; i < 6; i++)
            {
                if (!Int32.TryParse(Parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out P[i])) return false;
                if ((P[i] < 0) || (P[i] > 255)) return false;
            }

            UInt64 Temp = 0;
            for (int i = 0; i < 6; i++)
            {
                Temp |= (Convert.ToUInt64(P[5 - i]) << 8 * i);
            }

            A.Address = Temp;
            return true;
        }

        public void Increment()
        {
            Address++;
        }

        public override string ToString()
        {
            byte[] Bytes = new byte[6];
            for (int i = 0; i < 6; i++)
                Bytes[5 - i] = Convert.ToByte(Address >> (i * 8) & 0xFF);

            return String.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                Bytes[0], Bytes[1], Bytes[2], Bytes[3], Bytes[4], Bytes[5]);
        }
    }
}
