using Agent.Model;

namespace Agent.Enums
{
    public struct StatusMachine // Статус машины
    {
        public static event SetStat StatusChange;
        private bool m_wait;
        private bool m_initiator;
        private bool m_calculate;
        private bool m_testing;
        private bool m_loadSettings;
        private bool m_waitEndCalc;

        public bool Free { get; private set; }
        public bool Wait
        {
            get
            {
                return m_wait;
            }
            set
            {
                m_wait = value;
                checkFree();
                StatusChange();
            }
        }
        public bool Initiator
        {
            get
            {
                return m_initiator;
            }
            set
            {
                m_initiator = value;
                checkFree();
                StatusChange();
            }
        }
        public bool Calculate
        {
            get
            {
                return m_calculate;
            }
            set
            {
                m_calculate = value;
                checkFree();
                StatusChange();
            }
        }
        public bool Testing
        {
            get
            {
                return m_testing;
            }
            set
            {
                m_testing = value;
                checkFree();
                StatusChange();
            }
        }
        public bool LoadSettings
        {
            get
            {
                return m_loadSettings;
            }
            set
            {
                m_loadSettings = value;
                checkFree();
                StatusChange();
            }
        }
        public bool WaitEndCalc
        {
            get
            {
                return m_waitEndCalc;
            }
            set
            {
                m_waitEndCalc = value;
                checkFree();
                StatusChange();
            }
        }

        private void checkFree()
        {
            if (Wait || Initiator || Calculate || Testing || LoadSettings || WaitEndCalc)
                Free = false;
            else
                Free = true;
        }
        public string GetStatus()
        {
            if (Testing)
                return "Тестирование";
            if (LoadSettings)
                return "Загрузка настроек";
            if (Calculate)
                return "Считает";
            if (WaitEndCalc)
                return "Ждем окончания вычислений";
            if (Wait)
                return "Ждем команд";
            if (Initiator)
                return "Инициатор";
            if (Free)
                return "Свободен";
            return "Ошибка!!!";
        }
    }
}