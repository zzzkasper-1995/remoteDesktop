using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AdminServer
{
    public class Repeater
    {
        public static NetworkStream networkStream;
        public static StreamReader readerStream;
        public static TcpClient eClient;

        private static bool connectServer = false;
        private static bool connectUVNC = false;

        public static string IpRepeaterServer = "";
        public static int PortRepeaterServer = 34001;

        public static string IpUVNC = "";
        public static string PortUVNC = "";
        public static string wayToUVNC = "";

        // Объявляем делегат
        public delegate void delegateChangeStateConnectServer();
        // Событие, возникающее при изменеии состояния подключения к серверу или RepeaterUVNC
        public static event delegateChangeStateConnectServer EventChangeStateConnectServer = delegate { };

        public static bool ConnectServer
        {
            get { return connectServer; }
            set
            {
                connectServer = value;
                EventChangeStateConnectServer?.Invoke();
            }
        }

        public static bool ConnectUVNC
        {
            get { return connectUVNC; }
            set
            {
                connectUVNC = value;
                EventChangeStateConnectServer?.Invoke();
            }
        }

        public static void IsConnectToServer()//метод отслеживает доступность сервера
        {
            bool ConnectIsChange = true;

            while (true)
            {
                Thread.Sleep(1000);

                try
                {
                    IpRepeaterServer = Program.ReadSetting("IpRepeaterServer");
                    if (IpRepeaterServer == "" || IpRepeaterServer == null) IpRepeaterServer = Properties.Settings.Default.defaultIpRepeaterServer;
                }
                catch (Exception exc)
                {
                    Program.logger.Error("isConnectToServer() ошибка:" + exc);
                    IpRepeaterServer = Properties.Settings.Default.defaultIpRepeaterServer;
                }

                try
                {
                    if(Program.ReadSetting("PortRepeaterServer") !="")
                        PortRepeaterServer = Convert.ToInt32(Program.ReadSetting("PortRepeaterServer"));
                    else
                        PortRepeaterServer = Properties.Settings.Default.defaultPortRepeaterServer;
                }
                catch (Exception exc)
                {
                    PortRepeaterServer = Properties.Settings.Default.defaultPortRepeaterServer;
                }

                if (Program.login != "" && Program.pas != "")
                    if (eClient != null)
                    {
                        try
                        {
                            networkStream.Write(Encoding.UTF8.GetBytes(" "), 0, 1);

                            if (ConnectIsChange)
                            {
                                Request req = new Request("Hello");
                                req.parameters.Add(Program.login);
                                req.parameters.Add(Program.pas);
                                req.Send();
                            }
                            Thread.Sleep(500);

                            if (ConnectServer == true) { ConnectIsChange = false; }
                            else { ConnectIsChange = true; }
                            ConnectServer = true;
                        }
                        catch (Exception exc)
                        {
                            Program.logger.Debug("isConnectToServer() " + exc);
                            ConnectServer = false;
                            try
                            {
                                eClient = new TcpClient(IpRepeaterServer, PortRepeaterServer);
                                readerStream = new StreamReader(eClient.GetStream());
                                networkStream = eClient.GetStream();

                                if (ConnectServer == false) { ConnectIsChange = true; }
                                else { ConnectIsChange = false; }
                                ConnectServer = true;
                            }
                            catch (Exception exc1)
                            {
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            eClient = new TcpClient(IpRepeaterServer, PortRepeaterServer);
                            readerStream = new StreamReader(eClient.GetStream());
                            networkStream = eClient.GetStream();

                            Request req = new Request("Hello");
                            req.parameters.Add(Program.login);
                            req.parameters.Add(Program.pas);
                            req.Send();
                            Thread.Sleep(500);

                            if (ConnectServer == true) { ConnectIsChange = false; }
                            else { ConnectIsChange = true; }
                            ConnectServer = true;
                        }
                        catch (Exception exc)
                        {
                            Program.logger.Debug("isConnectToServer() " + exc);
                            if (ConnectServer == false) { ConnectIsChange = false; }
                            else { ConnectIsChange = true; }
                            ConnectServer = false;
                        }
                    }

                Thread.Sleep(500);
            }
        }
  
        public static void IsConnectToRepeaterUVNC()//метод проверяет доступность UVNC Repeater
        {
            while (true)
            {
                try
                {
                    IpUVNC = Program.ReadSetting("IpRepeaterUVNC");
                    if(IpUVNC=="" || IpUVNC==null)
                        IpUVNC = Properties.Settings.Default.defaultIpRepeaterUVNC;
                }
                catch (Exception exc)
                {
                    IpUVNC = Properties.Settings.Default.defaultIpRepeaterUVNC;
                }

                try
                {
                    if (Program.ReadSetting("PortRepeaterUVNC") != "")
                        PortUVNC = Program.ReadSetting("PortRepeaterUVNC");
                    else
                        PortUVNC = Convert.ToString(Properties.Settings.Default.defaultPortRepeaterUVNC);
                }
                catch (Exception exc)
                {
                    PortUVNC = Convert.ToString(Properties.Settings.Default.defaultPortRepeaterUVNC);
                }

                try
                {
                    wayToUVNC = Program.ReadSetting("WayToVNC");
                    if (wayToUVNC == "" || wayToUVNC == null)
                        wayToUVNC = Properties.Settings.Default.defaultWayToVNC;
                }
                catch (Exception exc)
                {
                    wayToUVNC = Properties.Settings.Default.defaultWayToVNC;
                }

                try
                {
                    Ping ping = new Ping();
                    PingReply pingReply = null;
                    pingReply = ping.Send(IpUVNC);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        ConnectUVNC = true;
                    }
                    else
                    {
                        ConnectUVNC = false;
                    }
                }
                catch (Exception exc)
                {
                    ConnectUVNC = false;
                }
                Thread.Sleep(500);
            }
        }
    }
}
