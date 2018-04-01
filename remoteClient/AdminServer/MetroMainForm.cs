using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Dynamic;
using MetroFramework;
using MetroFramework.Controls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace AdminServer
{
    public partial class MetroMainForm : MetroForm
    {
        public static RemoteClient remoteC = new RemoteClient(); //текущий удаленный клиент
        private static string findID = ""; //Искомый ID 
        private static bool isFindID = false; //Произошел ли поиск клиента
        private static ToolTip toolTipStatusLink; //Всплывающая подсказка
        private static ToolTip toolTipAddress; //Всплывающая подсказка адреса
        private static ToolTip toolTipProc; //Всплывающая подсказка процессора
        private static MetroListClientsForm ListForm=null; //Форма ЛистФорм
        private static List<string> listClientsID = new List<string>(); //Список подключенный к серверу Репитеру клиентов
        private static List<Lid> listLid = new List<Lid>(); //Список всех лидов
        private static bool uvncViewerWorking = false; //запущен ли сейчас UVNC Viewer

        // Объявляем делегат
        public delegate void delegateChangeUvncViewerWorking();
        public delegate void delegateChangeFindID();

        // Событие, возникающее при изменеии UvncViewerWorking
        public static event delegateChangeUvncViewerWorking EventChangeUvncViewerWorking = delegate { };
        // Событие, возникающее при изменеии UvncViewerWorking
        public static event delegateChangeFindID EventChangeFindID = delegate { };

        public static bool UvncViewerWorking
        {
            get { return uvncViewerWorking; }
            set
            {
                uvncViewerWorking = value;
                EventChangeUvncViewerWorking?.Invoke();
            }
        }

        public bool IsFindID
        {
            get { return isFindID; }
            set
            {
                isFindID = value;
                EventChangeFindID?.Invoke();
            }
        }

        public string FindID
        {
            get { return findID; }
            set
            {
                findID = value;
                EventChangeFindID?.Invoke();
            }
        }

        public static List<string> ListClientsID
        {
            get { return listClientsID; }
            set
            {
                if (!listClientsID.SequenceEqual(value))
                {
                    listClientsID = value;
                    if (ListForm != null)
                    {
                        try
                        {
                            ListForm.UpdataGridView_online();
                            ListForm.UpdataGridView_lid_stat();
                        }
                        catch { }
                    }
                    Program.MainForm.UpdateClientStat();
                }
            }
        }

        public static List<Lid> ListLid
        {
            get { return listLid; }
            set
            {
                if (!listLid.SequenceEqual(value))
                {
                    listLid = value;
                    if (ListForm != null)
                    {
                        try
                        {
                            ListForm.UpdataGridView_lid();
                        }
                        catch { }
                    }
                }
            }
        }

        public MetroMainForm()
        {
            // Create the ToolTip and associate with the Form container.
            toolTipStatusLink = new ToolTip();
            toolTipAddress = new ToolTip();
            toolTipProc = new ToolTip();

            InitializeComponent();

            Thread isConnectToRepeaterUVNCThread = new Thread(new ThreadStart(Repeater.IsConnectToRepeaterUVNC));//Поток для графического отображения состояния соединения с сервером
            isConnectToRepeaterUVNCThread.Start();

            Thread generalThread = new Thread(new ThreadStart(General));//основной рабочий поток программы
            generalThread.Start();

            Thread ConnectionTestThread = new Thread(new ThreadStart(UpdateListesLid));//Поток для графического отображения состояния соединения с сервером
            ConnectionTestThread.Start();

            Thread LookToUvncViewerThread = new Thread(new ThreadStart(LookToUvncViewer));//Поток для графического отображения состояния соединения с сервером
            LookToUvncViewerThread.Start();

            RemoteClient.EventChangeStatusConnect += UpdateLinkStatusConnect;
            EventChangeUvncViewerWorking += UpdateLinkStatusConnect;
            EventChangeUvncViewerWorking += UpdateTheme;
            Repeater.EventChangeStateConnectServer += UpdateTheme;
            Repeater.EventChangeStateConnectServer += UpdateLinkStatusConnect;
            Program.EventChangeCurrentVersion += UpdateCurrentVersion;
            EventChangeFindID += UpdateClientPersonInfo;
            RemoteClient.EventChangeClientSystemInfo += UpdateClientSystemInfo;
            RemoteClient.EventChangeStatusOnOrOff += UpdateStatusOnOrOffinForm;
        }

        private static void General()//основной метод который отвечает за работу с репитером
        {
            while (true)
            { if (Repeater.eClient != null) break; else Thread.Sleep(500); }

            try
            {
                Run();
            }
            catch (Exception exc)
            {
                Program.logger.Debug("general() " + exc);
                General();
            }
            finally { }
        }

        private static void Run()//метод который принимает запросы от клиентов
        {
            try
            {
                while (true)
                {
                    if (Repeater.ConnectServer)
                    {
                        try
                        {
                            string returnData = Repeater.readerStream.ReadLine();
                            returnData = Decrypt(returnData, "1234");

                            if (returnData != null && returnData != "")
                            {
                                Program.logger.Info("Получили запрос: " + returnData);
                                Response r = JsonConvert.DeserializeObject<Response>(returnData);
                                r.WorkWithResponse();
                            }
                        }
                        catch (Exception exc)
                        {
                            Program.logger.Debug("run() " + exc);
                        }
                    }
                }
            }
            catch (Exception exc) { Program.logger.Debug("run() " + exc); Run(); }
            finally
            {
                // Закрыть TcpClient
                if (Repeater.eClient != null) Repeater.eClient.Close();
            }
        }

        public void UpdateLinkStatusConnect()//обновляем текст и всплыввающую подсказу для строки состояния
        {
            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    if (remoteC.StatusConnect == "noConnect" || (!UvncViewerWorking && remoteC.StatusConnect == "Запуск UVNC"))
                    {
                        if (Repeater.ConnectServer && Repeater.ConnectUVNC)
                            if (toolTipStatusLink.ToolTipTitle != "Соединение установленно")
                            {
                                toolTipStatusLink.SetToolTip(isCon, "Соединение установленно");
                                isCon.Text = "Соединение установленно";
                            }
                        if (Repeater.ConnectServer && !Repeater.ConnectUVNC)
                            if (toolTipStatusLink.ToolTipTitle != "Сервер UVNC недоступен")
                            {
                                toolTipStatusLink.SetToolTip(isCon, "Сервер UVNC недоступен");
                                isCon.Text = "Сервер UVNC недоступен";
                            }
                        if (!Repeater.ConnectServer && Repeater.ConnectUVNC)
                            if (toolTipStatusLink.ToolTipTitle != "Сервер RemoteDesktop недоступен")
                            {
                                toolTipStatusLink.SetToolTip(isCon, "Сервер RemoteDesktop недоступен");
                                isCon.Text = "Сервер RemoteDesktop недоступен";
                            }
                        if (!Repeater.ConnectServer && !Repeater.ConnectUVNC)
                            if (toolTipStatusLink.ToolTipTitle != "Сервера UVNC и RemoteDesktop недоступны")
                            {
                                toolTipStatusLink.SetToolTip(isCon, "Сервера UVNC и RemoteDesktop недоступны");
                                isCon.Text = "Сервера UVNC и RemoteDesktop недоступны";
                            }
                    }
                    else
                    {
                        if (Repeater.ConnectServer && Repeater.ConnectUVNC)
                            if (toolTipStatusLink.ToolTipTitle != remoteC.StatusConnect)
                            {
                                toolTipStatusLink.SetToolTip(isCon, remoteC.StatusConnect);
                                isCon.Text = remoteC.StatusConnect;
                            }

                        if (UvncViewerWorking && remoteC.StatusConnect == "Запуск UVNC")
                        {
                            toolTipStatusLink.SetToolTip(isCon, "UVNC подключен к клиенту");
                            isCon.Text = "UVNC подключен к клиенту";
                        }
                    }
                }));
            }
            catch (Exception exc) { Program.logger.Debug("UpdateLinkStatusConnect() " + exc); }
        }

        public void UpdateTheme()//Обновляет тему окна и строку состояния
        {
            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    if (!Repeater.ConnectUVNC)
                    {
                        if (isCon.Style != MetroColorStyle.Red)
                        {
                            Style = MetroColorStyle.Red;
                            metroTabControl1.Style = MetroColorStyle.Red;
                            isCon.Style = MetroColorStyle.Red;
                            if (ListForm != null) ListForm.Style = MetroColorStyle.Red;
                            if (!Repeater.ConnectServer)
                            {
                                button_find.Enabled = false;
                                button_ConnectToRemovePC.Enabled = false;
                            }
                            else
                            {
                                button_ConnectToRemovePC.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        if (Repeater.ConnectServer)
                        {
                            if (isCon.Style != MetroColorStyle.Green)
                            {
                                Style = MetroColorStyle.Green;
                                metroTabControl1.Style = MetroColorStyle.Green;
                                isCon.Style = MetroColorStyle.Green;
                                isCon.Text = "Соединение установленно";

                                if (ListForm != null) ListForm.Style = MetroColorStyle.Green;

                                button_find.Enabled = true;
                                button_ConnectToRemovePC.Enabled = true;
                            }
                        }
                        else
                        {
                            if (isCon.Style != MetroColorStyle.Red)
                            {
                                Style = MetroColorStyle.Red;
                                metroTabControl1.Style = MetroColorStyle.Red;
                                isCon.Style = MetroColorStyle.Red;
                                if (ListForm != null) ListForm.Style = MetroColorStyle.Red;

                                button_find.Enabled = false;
                                button_ConnectToRemovePC.Enabled = false;
                            }
                        }
                    }
                }));
            }
            catch (Exception exc) { Program.logger.Debug("GIUpdate() " + exc); }
        }

        private void LookToUvncViewer()//следит за тем запущен ли UvncViewer
        {
            while (true)
            {
                Process[] pr = Process.GetProcessesByName(Properties.Settings.Default.UvncStartFile);
                if (pr.Length > 0)
                {
                    if (!UvncViewerWorking) UvncViewerWorking = true;
                }
                else
                {
                    if (UvncViewerWorking) UvncViewerWorking = false;
                }
            }
        }

        private void UpdateClientSystemInfo()//метод обновляет поля на форме с информацией о компьютере
        {
            if (FindID != "")
            {
                if (isFindID)
                {
                    try
                    {
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            if (remoteC.StatusOnOrOff)
                            {
                                if (label_OSVersion.Text != "ОС: " + remoteC.osVersion)
                                    label_OSVersion.Text = "ОС: " + remoteC.osVersion;

                                if (label_proc.Text != "Проессор: " + remoteC.procName)
                                {
                                    label_proc.Text = "Проессор: " + remoteC.procName;
                                    toolTipProc.SetToolTip(label_proc, remoteC.procName);
                                }

                                if (label_mem.Text != "ОЗУ: " + remoteC.physicalMemory)
                                    label_mem.Text = "ОЗУ: " + remoteC.physicalMemory;

                                if (label_mac.Text != "MAC: " + remoteC.Mac)
                                    label_mac.Text = "MAC: " + remoteC.Mac;
                            }
                            else
                            {
                                if (remoteC.osVersion == "")
                                    label_OSVersion.Text = "";
                                if (remoteC.procName == "")
                                {
                                    label_proc.Text = "";
                                    toolTipProc.SetToolTip(label_proc, "");
                                }
                                if (remoteC.physicalMemory == "")
                                    label_mem.Text = "";
                                if (remoteC.Mac == "")
                                    label_mac.Text = "";
                            }
                        }));
                    }
                    catch (Exception exc) { Program.logger.Debug("UpdateClientSystemInfo() " + exc); }
                }
            }
        }

        private void UpdateStatusOnOrOffinForm()//Обновляет поля клиента на форме которые зависят от его статуса Онлайн/Оффлайн
        {
            if (FindID != "")
            {
                if (isFindID)
                {
                    try
                    {
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            if (remoteC.StatusOnOrOff)
                            {
                                if (remoteC.Mac == "")
                                {
                                    Request req = new Request("GetSystemInfo");
                                    req.parameters.Add(FindID);
                                    req.Send();
                                }

                                button_ConnectToRemovePC.Visible = true;
                                button_ConnectToRemovePC.Enabled = true;
                                pictureBox_stat.Image = Properties.Resources.online;
                                label_stat.Text = "Online";
                            }
                            else
                            {
                                pictureBox_stat.Image = Properties.Resources.offline;
                                label_stat.Text = "Offline";
                            }
                        }));
                    }
                    catch (Exception exc) { Program.logger.Debug("UpdateStatusOnOrOffinForm() " + exc); }
                }
            }
        }

        private void UpdateClientPersonInfo()//Обновить информацию в полях на форме о компьютере клиента 
        {
            if (FindID != "")
            {
                if (isFindID)
                {
                    try
                    {
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            groupBoxGeneralInfo.Visible = true;
                            WriteLabelInfo();//Заполняем поля о лиде, инфу берем из битрикса

                            if (!remoteC.StatusOnOrOff)
                            {
                                if (remoteC.osVersion == "")
                                    label_OSVersion.Text = "";
                                if (remoteC.procName == "")
                                {
                                    label_proc.Text = "";
                                    toolTipProc.SetToolTip(label_proc, "");
                                }
                                if (remoteC.physicalMemory == "")
                                    label_mem.Text = "";
                                if (remoteC.Mac == "")
                                    label_mac.Text = "";
                            }
                        }));
                    }
                    catch (Exception exc) { Program.logger.Debug("UpdateClientSystemInfo() " + exc); }
                }
            }
        }

        public void UpdateCurrentVersion()//обновляет на форме поле Current version
        {
            try
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    if (label_version.Text != "Текущая версия программы: " + Program.CurrentVersion)
                    {
                        label_version.Text = "Текущая версия программы: " + Program.CurrentVersion;
                    }
                }));
            }
            catch { }
        }

        private void UpdateListesLid()//Обновляем списки всех лидов и лидов Online
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts = DateTime.Now - dt;

            while (true)
            {
                GetListClients();

                if (ts.Seconds >= 60)
                {
                    dt = DateTime.Now;
                    GetListLid();
                }
                ts = DateTime.Now - dt;

                Thread.Sleep(2000);
            }
        }

        public static bool ListEquals<T>(List<string> target, List<string> source)//сравниваем два списка
        {
            if (target.Count != source.Count)
                return false;
            else
            {
                for (int i = 0; i < target.Count; i++)
                {
                    if (!target[i].Equals(source[i]))
                        return false;
                }
                return true;
            }
        }

        private void UpdateClientStat()//Метод обновляет статус текущего клиента (Online или offline)
        {
            string id = ListClientsID.Find(x => x.Equals(FindID));
            if (id != null) { remoteC.StatusOnOrOff = true; return; }
            else { remoteC.StatusOnOrOff = false; }
        }

        public void ChooseFolder(MetroTextBox t)//метод помогает использовать диалог выбора папки
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                t.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public void FindId(string findID)//Начинает поиск лида по ID
        {
            if (textBox_find.Text != Program.MainForm.FindID)
            {
                remoteC.osVersion = "";
                remoteC.physicalMemory = "";
                remoteC.procName = "";
                remoteC.Mac = "";
            }

            textBox_find.Text = findID;
            FindID = findID;

            if (Repeater.ConnectServer)
            {
                remoteC.StatusConnect = "Идет поиск клиента";
                string TaskListByJSON = Program.bx_logon.SendCommand("crm.lead.get", "ID=" + findID);

                if (TaskListByJSON != null)
                {
                    try
                    {
                        WriteInfoFromBitrix(TaskListByJSON);
                        Request req = new Request("Find");
                        req.parameters.Add(findID);
                        req.Send();
                    }
                    catch (Exception exc)
                    {
                        Program.logger.Debug("FindId() " + exc);
                        MessageBox.Show("Пользователь с таким ID  в системе не найден", "Поиск");
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким ID  в системе не найден", "Поиск");
                }
            }
            else
            {
                MessageBox.Show("Нельзя выполнить эту команду пока нет соединения с сервером", "Ошибка");
            }
        }
  
        public void ApplyOptions()//Метод применяет указанные настройки для программы
        {
            List<string> txt = new List<string>();
            if (textBox_WayToVNC.Text != Repeater.wayToUVNC) //если был изменен путь к UVNC
            {
                if (textBox_WayToVNC.Text != "")//путь до UVNC
                {
                    Program.AddUpdateAppSettings("WayToVNC", textBox_WayToVNC.Text);
                }
                else
                {
                    textBox_WayToVNC.Text = Repeater.wayToUVNC;
                }
            }

            if (textBox_IpPortRepeaterUVNC.Text != Repeater.IpUVNC + ":" + Repeater.PortUVNC) //Если был изменен IP или порт uvnc
            {
                bool badChange = true;
                if (textBox_IpPortRepeaterUVNC.Text != "")//адрес UVNC репитера
                {
                    if (textBox_IpPortRepeaterUVNC.Text.Split(':').Length == 2)
                    {
                        if (textBox_IpPortRepeaterUVNC.Text.Split(':')[0] != "" && textBox_IpPortRepeaterUVNC.Text.Split(':')[1] != "")
                        {
                            try
                            {
                                Program.AddUpdateAppSettings("IpRepeaterUVNC", textBox_IpPortRepeaterUVNC.Text.Split(':')[0]);
                                Program.AddUpdateAppSettings("PortRepeaterUVNC", textBox_IpPortRepeaterUVNC.Text.Split(':')[1]);
                                badChange = false;
                            }
                            catch
                            {
                                badChange = true;
                            }
                        }
                    }
                }

                if (badChange)
                {
                    textBox_IpPortRepeaterUVNC.Style = MetroColorStyle.Red;
                }
            }

            if (textBox_IpPortRepeaterServer.Text != Repeater.IpRepeaterServer + ":" + Repeater.PortRepeaterServer)// если был изменен IP или порт сервера репитера
            {
                bool badChange = true;
                if (textBox_IpPortRepeaterServer.Text != "")//адрес сервера 
                {
                    if (textBox_IpPortRepeaterServer.Text.Split(':').Length == 2)
                    {
                        if (textBox_IpPortRepeaterServer.Text.Split(':')[0] != "" && textBox_IpPortRepeaterServer.Text.Split(':')[1] != "")
                        {
                            try
                            {
                                bool a = true, b = true;
                                a = Program.AddUpdateAppSettings("IpRepeaterServer", textBox_IpPortRepeaterServer.Text.Split(':')[0]);
                                b = Program.AddUpdateAppSettings("PortRepeaterServer", textBox_IpPortRepeaterServer.Text.Split(':')[1]);
                                badChange = false;
                                if( !a || !b) badChange = true;
                                else
                                {
                                    Request req = new Request("Exit");
                                    req.Send();
                                }
                            }
                            catch
                            {
                                badChange = true;
                            }
                        }
                    }
                }

                if (badChange)
                {
                    textBox_IpPortRepeaterServer.Style = MetroColorStyle.Red;
                }
            }

            if (textBox_IpPortRepeaterUVNC.Style == MetroColorStyle.Red || textBox_IpPortRepeaterServer.Style == MetroColorStyle.Red)
            {
                MessageBox.Show("Не верный формат ввода");
            }
            else
            {
                metroTabControl1.SelectedTab = metroTabPage1; //переключаемся на вкладку "Удаленный клиент"
            }
        }

        public static void GetListClients()//Получить список текущих подключенных к серверу клиентов
        {
            Request req = new Request("GetListClients");
            req.Send();
        }

        public static void GetListLid()//Получаем список всех лидов из базы битрикса
        {
            try
            {
                string TaskListByJSON = Program.bx_logon.SendCommand("crm.lead.list", "ORDER[DATE_CREATE]=ASC&FILTER[]=&SELECT[]=NAME&SELECT[]=SECOND_NAME&SELECT[]=UF_CRM_1513083163&SELECT[]=LAST_NAME&SELECT[]=PHONE");
                var converter = new ExpandoObjectConverter();

                dynamic res = null;
                int resCount = 0;

                if (TaskListByJSON != null)
                {
                    res = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);
                    resCount = res.result.Count;
                }
                else { }

                try
                {
                    for (int i = 0; i < resCount; i++)
                    {
                        string ID, NAME, ADDRESS, SECOND_NAME, LAST_NAME, PHONE;

                        Lid l = new Lid();
                        try
                        {
                            if (res.result[i].ID == "" || res.result[i].ID == null) l.ID = "";
                            else l.ID = res.result[i].ID;
                        }
                        catch { l.ID = ""; }

                        try
                        {
                            if (res.result[i].NAME == "" || res.result[i].NAME == null) l.NAME = "";
                            else l.NAME = res.result[i].NAME;
                        }
                        catch { l.NAME = ""; }

                        try
                        {
                            if (res.result[i].UF_CRM_1513083163 == "" || res.result[i].UF_CRM_1513083163 == null) l.ADDRESS = "";
                            else l.ADDRESS = res.result[i].UF_CRM_1513083163;

                            for (int j = 0; j < l.ADDRESS.Length - 7; j++)
                            {
                                string a = "" + l.ADDRESS[j] + l.ADDRESS[j + 1] + l.ADDRESS[j + 2] + l.ADDRESS[j + 3] + l.ADDRESS[j + 4] + l.ADDRESS[j + 5];
                                if (a == "Россия")
                                { l.ADDRESS = l.ADDRESS.Remove(j + 6); }
                            }
                        }
                        catch { l.ADDRESS = ""; }

                        try
                        {
                            if (res.result[i].SECOND_NAME == "" || res.result[i].SECOND_NAME == null) l.SECOND_NAME = "";
                            else l.SECOND_NAME = res.result[i].SECOND_NAME;
                        }
                        catch { l.SECOND_NAME = ""; }

                        try
                        {
                            if (res.result[i].LAST_NAME == "" || res.result[i].LAST_NAME == null) l.LAST_NAME = "";
                            else l.LAST_NAME = res.result[i].LAST_NAME;
                        }
                        catch { l.LAST_NAME = ""; }

                        try
                        {
                            if (res.result[i].PHONE[0].VALUE == "" || res.result[i].PHONE[0].VALUE == null) l.PHONE = "";
                            else l.PHONE = res.result[i].PHONE[0].VALUE;
                        }
                        catch (Exception exp) { l.PHONE = ""; }
                        listLid.Add(l);
                    }
                }
                catch { }
            }
            catch (Exception exc) { }
        }

        public void WriteInfoFromBitrix(string TaskListByJSON)//Преобразовать JSON  с информацией о лиде в поля RemoteClient
        {
            if (TaskListByJSON != null)
            {
                var converter = new ExpandoObjectConverter();
                dynamic res = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);
                if (res.result.ID == FindID)
                {
                    remoteC.id = FindID;

                    try
                    {
                        if (res.result.NAME == "" || res.result.NAME == null) remoteC.name = "unknown";
                        else remoteC.name = res.result.NAME;
                    }
                    catch { remoteC.name = "unknown"; }

                    try
                    {
                        if (res.result.UF_CRM_1513083163 == "" || res.result.UF_CRM_1513083163 == null) remoteC.address = "unknown";
                        else remoteC.address = res.result.UF_CRM_1513083163;

                        for (int i = 0; i < remoteC.address.Length - 7; i++)
                        {
                            string a = "" + remoteC.address[i] + remoteC.address[i + 1] + remoteC.address[i + 2] + remoteC.address[i + 3] + remoteC.address[i + 4] + remoteC.address[i + 5];
                            if (a == "Россия")
                            { remoteC.address = remoteC.address.Remove(i + 6); }
                        }
                    }
                    catch { remoteC.address = "unknown"; }

                    try
                    {
                        if (res.result.SECOND_NAME == "" || res.result.SECOND_NAME == null) remoteC.second_name = "unknown";
                        else remoteC.second_name = res.result.SECOND_NAME;
                    }
                    catch { remoteC.second_name = "unknown"; }

                    try
                    {
                        if (res.result.LAST_NAME == "" || res.result.LAST_NAME == null) remoteC.last_name = "unknown";
                        else remoteC.last_name = res.result.LAST_NAME;
                    }
                    catch { remoteC.last_name = "unknown"; }

                    try
                    {
                        if (res.result.PHONE[0].VALUE == "" || res.result.PHONE[0].VALUE == null) remoteC.phone = "unknown";
                        else remoteC.phone = res.result.PHONE[0].VALUE;
                    }
                    catch (Exception exp) { remoteC.phone = "unknown"; }
                }
            }
            else
            {
                remoteC.phone = "unknown";
                remoteC.last_name = "unknown";
                remoteC.second_name = "unknown";
                remoteC.address = "unknown";
                remoteC.name = "unknown";
            }
        }

        public void ConnectToClient(string id) //начинаем подключение к клиенту по ID
        {
            if (id != "")
            {
                if (MessageBox.Show("Вы точно хотите подключиться к клиенту с ID:" + id + "?",
                                    "NEO-SERVICE24",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Request req = new Request("Connect");
                    req.parameters.Add(id);
                    req.Send();
                }
            }
            else
            {
                MessageBox.Show("Что то пошло не так, возможно пользователь уже недоступен", "Удаленная помощь с компьютером", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearLabelInfo()//очистить поля информации о клиенте
        {
            label_id.Text = "";
            label_second_name.Text = "";
            linkLabel_link.Text = "";
            label_name.Text = "";
            label_number.Text = "";
            label_address.Text = ""; toolTipAddress.SetToolTip(label_address, "");
            label_OSVersion.Text = "";
            label_proc.Text = ""; toolTipProc.SetToolTip(label_proc, "");
            label_mem.Text = "";
            label_mac.Text = "";
        }

        public void WriteLabelInfo()//Заполняем поля о лиде, инфу берем из битрикса
        {
            if (label_id.Text != "ID: "+ remoteC.id)
                label_id.Text = "ID: " + remoteC.id;

            if (linkLabel_link.Text != "neo-service24.bitrix24.ru/crm/lead/details/" + remoteC.id + "/")
                linkLabel_link.Text = "neo-service24.bitrix24.ru/crm/lead/details/" + remoteC.id + "/"; ;

            if (label_name.Text != "Имя: "+ remoteC.name)
                label_name.Text = "Имя: "+ remoteC.name;

            if (label_number.Text != "Телефон: "+ remoteC.phone)
                label_number.Text = "Телефон: "+ remoteC.phone;

            if (label_second_name.Text != "Фамилия: "+ remoteC.last_name)
                label_second_name.Text = "Фамилия: "+ remoteC.last_name;

            if (label_address.Text != "Адрес: " + remoteC.address)
            {
                label_address.Text = "Адрес: "+ remoteC.address;
                toolTipAddress.SetToolTip(label_address, remoteC.address);
            }
        }

        //метод дешифрования строки
        public static string Decrypt(string ciphText, string pass,string sol = "doberman",
                                     string cryptographicAlgorithm = "SHA1", int passIter = 2,
                                     string initVec = "a8doSuDitOz1hZe#", int keySize = 256)
        {
            if (string.IsNullOrEmpty(ciphText))
                return "";

            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] cipherTextBytes = Convert.FromBase64String(ciphText);

            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);

            RijndaelManaged symmK = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;

            using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
            {
                using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                    {
                        byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        mSt.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmK.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }

        //-------------------ОБРАБОТКА СОБЫТИЙ КОНТРОЛОВ--------------------------------------
        private void MetroMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Request req = new Request("Exit");
            req.Send();
            Process.GetCurrentProcess().Kill();
        }

        private void metroButton1_Click(object sender, EventArgs e)//сохраняем конфигурацию в файл по нажатию на кнопку применить
        {
            ApplyOptions();
        }

        private void textBox_WayToVNC_Click(object sender, EventArgs e)
        {
            ChooseFolder(textBox_WayToVNC);
        }

        private void textBox_WayToVNC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                textBox_WayToVNC.Text = Repeater.wayToUVNC;
            }

            if (e.KeyChar == 13)
            {
                ApplyOptions();
            }
            else
            {
                e.Handled = false;
            }
        }

        private void button_find_Click(object sender, EventArgs e)
        {
            FindID = textBox_find.Text;
            FindId(FindID);
        }

        private void button_ConnectToRemovePC_Click(object sender, EventArgs e)
        {
            ConnectToClient(textBox_find.Text);
        }

        private void textBox_find_TextChanged(object sender, EventArgs e)
        {
            if(textBox_find.Text!=remoteC.id) button_ConnectToRemovePC.Visible = false;
            else button_ConnectToRemovePC.Visible = true;

            isFindID = false;
            FindID = "";
        }

        private void textBox_find_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar >= 59) && e.KeyChar != 8)
            {
                e.Handled = true;
            }

            if (e.KeyChar == 13)
            {
                FindId(textBox_find.Text);
            }
        }

        private void MetroMainForm_Load(object sender, EventArgs e)
        {
            //Заполняем текстовые поля окна настроек
            textBox_WayToVNC.Text = Repeater.wayToUVNC;
            textBox_IpPortRepeaterUVNC.Text = Repeater.IpUVNC +":"+Repeater.PortUVNC;
            textBox_IpPortRepeaterServer.Text = Repeater.IpRepeaterServer +":"+ Repeater.PortRepeaterServer;
            label_version.Text = "Текущая версия программы: " + Program.ReadSetting("thisVersion");

            textBox_IpPortRepeaterUVNC.Style = MetroColorStyle.White;
            textBox_IpPortRepeaterServer.Style = MetroColorStyle.White;
            //---------------Закончили заполнять поля------------------

            metroTabControl1.SelectedTab = metroTabPage1; //переключаемся на вкладку "Удаленный клиент"
        }

        private void metroTabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //Заполняем текстовые поля окна настроек
            textBox_WayToVNC.Text = Repeater.wayToUVNC;
            textBox_IpPortRepeaterUVNC.Text = Repeater.IpUVNC + ":" + Repeater.PortUVNC;
            textBox_IpPortRepeaterServer.Text = Repeater.IpRepeaterServer + ":" + Repeater.PortRepeaterServer;

            textBox_IpPortRepeaterUVNC.Style = MetroColorStyle.White;
            textBox_IpPortRepeaterServer.Style = MetroColorStyle.White;
        }

        private void textBox_IpPortRepeaterUVNC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                textBox_WayToVNC.Text = Repeater.wayToUVNC;
            }
            if (e.KeyChar == 13)
            {
                ApplyOptions();
            }
        }

        private void textBox_IpPortRepeaterServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                textBox_IpPortRepeaterServer.Text = Repeater.IpRepeaterServer + ":" + Repeater.PortRepeaterServer;
            }

            if (e.KeyChar == 13)
            {
                ApplyOptions();
            }
        }

        private void MetroMainForm_LocationChanged(object sender, EventArgs e)
        {   
            if(ListForm!=null)
            {
                if (ListForm.Line)
                {
                    int x = Location.X + Size.Width;
                    int y = Location.Y;

                    Point XY = new Point(x, y);
                    ListForm.Location = XY;
                    ListForm.TopMost = true;
                }
                ListForm.TopMost = false;
            }

        }

        private void linkLabel_link_Click(object sender, EventArgs e)
        {
            Process.Start("https://neo-service24.bitrix24.ru/crm/lead/details/" + remoteC.id + "/");
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (ListForm == null)
            {
                ListForm = new MetroListClientsForm();
                ListForm.Show();
            }
            else
            {
                ListForm.Line = true;
                ListForm.MinimizeBox = false;
                int x = Location.X + Size.Width;
                int y = Location.Y;

                Point XY = new Point(x, y);
                ListForm.Location = XY;
                try
                {
                    if (ListForm.CanFocus)
                    {
                        ListForm.Focus();
                    }
                    ListForm.Show();
                }
                catch
                {
                    ListForm = new MetroListClientsForm();
                    ListForm.Show();
                }
            }
        }
        //-------------------КОНЕЦ ОБРАБОТКА СОБЫТИЙ КОНТРОЛОВ--------------------------------------
    }
}
