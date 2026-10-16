namespace NyaProg.Plugins.SettingsGen.Settings
{
    /// <summary>
    /// Обобщённый CRC (ширина 8/16/32) с настраиваемыми параметрами:
    /// полином (обычный, MSB), init, xorout, отражение входа/выхода.
    /// </summary>
    public static class Crc
    {
        public static ulong Compute(byte[] Data, int Width, ulong Poly, ulong Init, ulong XorOut, bool RefIn, bool RefOut)
        {
            ulong Mask = GetMask(Width);
            ulong Crc = Init & Mask;

            if (RefIn)
            {
                ulong RPoly = Reflect(Poly, Width);
                Crc = Reflect(Crc, Width);

                foreach (var B in Data)
                {
                    Crc ^= B;
                    for (int i = 0; i < 8; i++)
                        Crc = ((Crc & 1) != 0) ? ((Crc >> 1) ^ RPoly) : (Crc >> 1);
                    Crc &= Mask;
                }

                if (!RefOut) Crc = Reflect(Crc, Width);
            }
            else
            {
                int Top = Width - 1;

                foreach (var B in Data)
                {
                    Crc ^= (ulong)B << (Width - 8);
                    for (int i = 0; i < 8; i++)
                        Crc = ((Crc & (1UL << Top)) != 0) ? ((Crc << 1) ^ Poly) : (Crc << 1);
                    Crc &= Mask;
                }

                if (RefOut) Crc = Reflect(Crc, Width);
            }

            return (Crc ^ XorOut) & Mask;
        }

        private static ulong GetMask(int Width) => (Width >= 32) ? 0xFFFFFFFFUL : ((1UL << Width) - 1);

        private static ulong Reflect(ulong Value, int Width)
        {
            ulong Res = 0;

            for (int i = 0; i < Width; i++)
                if ((Value & (1UL << i)) != 0)
                    Res |= 1UL << (Width - 1 - i);

            return Res;
        }
    }
}
