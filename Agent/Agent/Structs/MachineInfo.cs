using System;

namespace Agent.Structs
{
    [Serializable]
    public struct MachineInfo
    {
        public long id;        // ID машины
        public int vRam;       // Объем RAM
        public int vCPU;       // Частота CPU
    }
}