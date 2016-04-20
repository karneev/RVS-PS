using Agent.Enums;
using Agent.Model;
using System;

namespace Agent.Structs
{
    [Serializable]
    public struct MachineInfo
    {
        [field: NonSerialized]
        public event SetStat StatusChange;
        public long id;        // ID машины
        public int vRam;       // Объем RAM
        public int vCPU;       // Частота CPU
        private StatusMachine status;    // Статус машины
        public StatusMachine Status
        {
            get
            {
                return this.status;
            }
            set
            {
                status = value;
                StatusChange(status);
            }
        }
    }
}