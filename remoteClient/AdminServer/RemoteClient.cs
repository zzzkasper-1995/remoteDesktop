namespace AdminServer
{
    public class RemoteClient //класс моделирующий удаленного клиента
    {
        public string osVersion = "";
        public string physicalMemory = "";
        public string procName = "";
        public string id = "";
        public string pas = "";
        public string pasEncrypted = "";
        public string address = "";
        public string name = "";
        public string second_name = "";
        public string last_name = "";
        public string phone = "";
        private string mac = "";

        private string statusConnect="noConnect";//Статус соединения
        private bool statusOnOrOff = false;

        // Объявляем делегат
        public delegate void delegateChangeStatusConnect();
        public delegate void delegateChangeClientSystemInfo();
        public delegate void delegateChangeStatusOnOrOff();

        // Событие, возникающее при изменеии statusConnect
        public static event delegateChangeStatusConnect EventChangeStatusConnect = delegate { };
        // Событие, возникающее при изменеии информации о компьютере клиента
        public static event delegateChangeClientSystemInfo EventChangeClientSystemInfo = delegate { };
        // Событие, возникающее при изменении статуса клиента подключения к серверу
        public static event delegateChangeStatusOnOrOff EventChangeStatusOnOrOff = delegate { };

        public string StatusConnect
        {
            get { return statusConnect; }
            set
            {
                statusConnect = value;
                EventChangeStatusConnect?.Invoke();
            }
        }

        public bool StatusOnOrOff
        {
            get { return statusOnOrOff; }
            set
            {
                statusOnOrOff = value;
                EventChangeStatusOnOrOff?.Invoke();
            }
        }

        public string Mac
        {
            get { return mac; }
            set
            {
                mac = value;
                EventChangeClientSystemInfo?.Invoke();
            }
        }

        public RemoteClient() { }
    }
}
