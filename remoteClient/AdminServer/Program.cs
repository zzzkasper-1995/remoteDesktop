using System;
using System.Windows.Forms;
using System.Configuration;
using NLog;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Security.Principal;
using System.ComponentModel;
using Newtonsoft.Json;

namespace AdminServer
{
    static class Program
    {
        public static Bitrix24 bx_logon = null; //токен битрикс
        public static Logger logger = LogManager.GetCurrentClassLogger();//отвечает за ведение лога программы
        public static string login = "";//логин пользователя
        public static string pas = "";//пароль пользователя
        public static short Auth = 0; //0 - не было попыток логиниться, 1 - попытка логиниться не удачная, 2 - пользователь ввел верный логин и пароль
        public static MetroMainForm MainForm;
        private static string currentVersion = ReadSetting("thisVersion");

        // Объявляем делегаты
        public delegate void delegateChangeCurrentVersion();
        // Событие, возникающее при изменении значения переменной хранящей текущую версию ПО 
        public static event delegateChangeCurrentVersion EventChangeCurrentVersion = delegate { };

        public static string CurrentVersion
        {
            get { return currentVersion; }
            set
            {
                currentVersion = value;
                EventChangeCurrentVersion?.Invoke();
            }
        }

        [STAThread]
        static void Main()
        {
            bool cn;

            //Проверяем запущена программа с правами администратора или нет
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (hasAdministrativeRight == false)
            {
                logger.Error("Main() if (hasAdministrativeRight == false)");
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Verb = "runas", //в данном случае указываем, что процесс должен быть запущен с правами администратора
                    FileName = Application.ExecutablePath //указываем исполняемый файл (программу) для запуска
                }; //создаем новый процесс
                try
                {
                    Process.Start(processInfo); //пытаемся запустить процесс
                }
                catch (Win32Exception)
                {
                    //Ничего не делаем, потому что пользователь, возможно, нажал кнопку "Нет" в ответ на вопрос о запуске программы в окне предупреждения UAC (для Windows 7)
                }
                Application.Exit(); //закрываем текущую копию программы (в любом случае, даже если пользователь отменил запуск с правами администратора в окне UAC)
            }
            else //имеем права администратора, значит, стартуем
            {
                logger.Error("Main() if (hasAdministrativeRight != false)");
                Mutex m = new Mutex(true, "AdminServerADS", out cn);
                if (!m.WaitOne(0, false))
                {
                    logger.Error("Main() if (!m.WaitOne(0, false))");
                    MessageBox.Show("Программа уже запущена на данном ПК");
                    return;
                }

                Thread ConnectThread = new Thread(new ThreadStart(Repeater.IsConnectToServer));//этот поток мониторит состояние подключения к серверу
                ConnectThread.Start();

                //update();
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MainForm = new MetroMainForm();
                Application.Run(new MetroAuthenticationForm());
            }
        }

        public static bool AddUpdateAppSettings(string key, string value)//Обновить или добавить новое поле пользовательских настроек
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                return true;
            }
            catch (ConfigurationErrorsException exc)
            {
                logger.Debug("AddUpdateAppSettings ошибка: "+ exc);
                MessageBox.Show("Ошибка записи настроек", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static string ReadSetting(string key)//счиатать значение пользовательских настроек
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? null;
                return result;
            }
            catch (ConfigurationErrorsException exc)
            {
                logger.Debug("ReadSetting ошибка: "+ exc);
                MessageBox.Show("Ошибка чтения настроек", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        //эта штука обновляет Updater и при необходимости запускает обновление
        public static int Update()
        {
            logger.Info("update()");

            string NewVersionClient = "";
            string NewVersionUpdater = "";
            string fileNameWithVersion = Properties.Settings.Default.fileNameWithVersion;

            string CurrentVersionClient = ReadSetting("thisVersion");
            string CurrentVersionUpdater = ReadSetting("VersionUpdater");
            string serverUpdater = Properties.Settings.Default.serverUpdater;
            string fileNameForUpdate = Properties.Settings.Default.Updater+".exe";

            try
            {
                //Проверяем доступность сервера обновлений
                WebRequest webRequestAccessHttp = WebRequest.Create(Properties.Settings.Default.serverUpdater);
                webRequestAccessHttp.Method = "HEAD";
                try
                {
                    using (WebResponse webResponse = webRequestAccessHttp.GetResponse())
                    {
                        Console.WriteLine("Сервер обновлений доступен");
                    }
                }
                catch (WebException exc)
                {
                    logger.Error("update() ошибка:" + exc);
                    Console.WriteLine("Сервер обновлений не доступен");
                    return 0;
                }

                //копируем файл с информацией о версии с сервера
                string FileithInfo = serverUpdater + "/" + fileNameWithVersion;
                var client = new WebClient();
                client.DownloadFile(FileithInfo, fileNameWithVersion);

                string str = "";
                foreach(string line in File.ReadLines(fileNameWithVersion))
                { str += line; }
                Response r = JsonConvert.DeserializeObject<Response>(str);

                //выясняем новую версию client и updater
                foreach (string line in r.parameters)
                {
                    if (line.Contains("VersionAdmin"))
                    {
                        NewVersionClient = "";
                        for (int i = "VersionAdmin".Length + 1; i < line.Length; i++)
                        {
                            NewVersionClient += line[i];
                        }
                        continue;
                    }
                    if (line.Contains("VersionUpdater"))
                    {
                        NewVersionUpdater = "";
                        for (int i = "VersionUpdater".Length + 1; i < line.Length; i++)
                        {
                            NewVersionUpdater += line[i];
                        }
                        continue;
                    }
                }
                File.Delete(fileNameWithVersion);

                if (Convert.ToDouble(CurrentVersionClient) < Convert.ToDouble(NewVersionClient)) //если обнаружена более новая версия программы
                {
                    MessageBox.Show("Обнаружена более новая версия программы. Приложение будет автоматически обновленно и перезапущенно", "AdminServer", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Process[] pr = Process.GetProcessesByName(Properties.Settings.Default.Updater);
                    if (pr.Length != 0)
                    {
                        foreach (Process p in pr)
                        {
                            p.Kill();
                        }
                        Thread.Sleep(2000);
                    }

                    //копируем файл с информацией о составе программы
                    string ListFiles = Properties.Settings.Default.ListFiles;
                    string myStringWebResource = Properties.Settings.Default.serverUpdater + "/" + ListFiles;
                    logger.Debug("Updater: " + myStringWebResource);
                    client.DownloadFile(myStringWebResource, ListFiles);

                    str = "";
                    foreach (string line in File.ReadLines(ListFiles))
                    { str += line; }
                    r = JsonConvert.DeserializeObject<Response>(str);

                    //проверяем наличие файлов на сервере
                    foreach (string lineFile in r.parameters)
                    {
                        myStringWebResource = Properties.Settings.Default.serverUpdater + "/UpdaterAdmin/" + lineFile;
                        logger.Debug("Updater: " + myStringWebResource);
                        WebRequest webRequest = WebRequest.Create(myStringWebResource);
                        webRequest.Method = "HEAD";
                        try
                        {
                            using (WebResponse webResponse = webRequest.GetResponse())
                            {
                                Console.WriteLine("Существует");
                            }
                        }
                        catch (WebException ex)
                        {
                            logger.Debug("При обновлении не удалось получить доступ к файлу на сервере. Ошибка: " + ex);
                            Console.WriteLine("Не существует: " + ex.Message);
                            return 0;
                        }
                    }

                    //если в предыдущем икле мы не вылетели значит все файлы на сервере на месте
                    foreach (string lineFile in r.parameters)
                    {
                        if (lineFile != "")
                        {
                            myStringWebResource = Properties.Settings.Default.serverUpdater + "/UpdaterAdmin/" + lineFile;
                            logger.Debug("Updater: " + myStringWebResource);
                            client.DownloadFile(myStringWebResource, lineFile);
                        }
                    }
                    File.Delete(ListFiles);
                    AddUpdateAppSettings("VersionUpdater", NewVersionUpdater);

                    ProcessStartInfo procInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = @"C:\Users\Программист\source\repos\NewRepo\remoteClient — копия (4)\update\bin\Debug\update.exe"//fileNameForUpdate; //The file in that DIR.
                    };
                    logger.Debug("Updater: " + procInfo.FileName);
                    procInfo.Verb = "runas";
                    Process ProcessUpdate = Process.Start(procInfo);  //Start that process.

                    Process.GetCurrentProcess().Kill();
                }
                return 1;
            }
            catch (Exception exc)
            {
                logger.Error("Updater: " + exc);
                Process.GetCurrentProcess().Kill();
                return 0;
            }
        }
    }
}
