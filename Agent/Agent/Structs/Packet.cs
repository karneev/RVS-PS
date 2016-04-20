using Agent.Enums;
using System;

namespace Agent.Structs
{
    [Serializable]
    public struct Packet // Пакет
    {
        public PacketType type;  // Код пакета
        public long id;          // ID источника пакета
        public override string ToString()
        {
            return type.ToString() + " " + id;
        }
    }
}