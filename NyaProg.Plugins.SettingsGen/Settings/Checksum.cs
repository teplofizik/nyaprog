namespace NyaProg.Plugins.SettingsGen.Settings
{
    /// <summary>Контрольные суммы.</summary>
    public static class Checksum
    {
        public static uint Sum(byte[] Data)
        {
            uint S = 0;
            foreach (var B in Data) S += B;
            return S;
        }

        /// <summary>CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF).</summary>
        public static ushort Crc16(byte[] Data)
        {
            ushort Crc = 0xFFFF;

            foreach (var B in Data)
            {
                Crc ^= (ushort)(B << 8);
                for (int i = 0; i < 8; i++)
                    Crc = (ushort)(((Crc & 0x8000) != 0) ? ((Crc << 1) ^ 0x1021) : (Crc << 1));
            }

            return Crc;
        }

        /// <summary>CRC-32 (poly 0xEDB88320, init 0xFFFFFFFF, xorout 0xFFFFFFFF).</summary>
        public static uint Crc32(byte[] Data)
        {
            uint Crc = 0xFFFFFFFF;

            foreach (var B in Data)
            {
                Crc ^= B;
                for (int i = 0; i < 8; i++)
                    Crc = ((Crc & 1) != 0) ? ((Crc >> 1) ^ 0xEDB88320) : (Crc >> 1);
            }

            return Crc ^ 0xFFFFFFFF;
        }
    }
}
