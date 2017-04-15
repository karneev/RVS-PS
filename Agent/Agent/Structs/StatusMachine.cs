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
        private bool m_connectedFail;

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
                CheckFree();
                StatusChange?.Invoke();
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
                CheckFree();
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
                CheckFree();
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
                CheckFree();
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
                CheckFree();
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
                CheckFree();
                StatusChange();
            }
        }
        public bool ConnectedFail
        {
            get
            {
                return m_connectedFail;
            }
            set
            {
                m_connectedFail = value;
                CheckFree();
                StatusChange();
            }
        }

        private void CheckFree()
        {
            if (Wait || Initiator || Calculate || Testing || LoadSettings || WaitEndCalc || ConnectedFail)
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
            if (ConnectedFail)
                return "Ошибка подключения";
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